using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FreeParkingMaui.Models;
using FreeParkingMaui.Services;
using FreeParkingMaui.Views;
using System.Timers;

namespace FreeParkingMaui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private readonly EmailService _emailService;
    private System.Timers.Timer _timer;
    private System.Timers.Timer _countdownTimer;
    private int _countdownSeconds = 600;

    [ObservableProperty]
    private string _userPhone;

    [ObservableProperty]
    private string _areaName;

    [ObservableProperty]
    private bool _isMonitoring;

    [ObservableProperty]
    private string _apiStatusText;

    [ObservableProperty]
    private string _lastCheckTime;

    [ObservableProperty]
    private string _nextCheckTime;

    [ObservableProperty]
    private string _countdownText;

    [ObservableProperty]
    private OrderData _latestOrder;

    [ObservableProperty]
    private bool _hasOrder;

    [ObservableProperty]
    private bool _hasTimeInfo;

    [ObservableProperty]
    private string _smtpUser;

    [ObservableProperty]
    private string _smtpPassword;

    private int _lastOrderState = -1;

    public IAsyncRelayCommand ShowEmailSettingsCommand { get; }

    public MainViewModel(ApiService apiService, EmailService emailService)
    {
        _apiService = apiService;
        _emailService = emailService;

        _timer = new System.Timers.Timer(10 * 60 * 1000); // 10 minutes
        _timer.Elapsed += async (s, e) => await CheckOrder();

        _countdownTimer = new System.Timers.Timer(1000);
        _countdownTimer.Elapsed += (s, e) => UpdateCountdown();

        ApiStatusText = "等待用户操作";
        CountdownText = "600s";

        ShowEmailSettingsCommand = new AsyncRelayCommand(ShowEmailSettingsAsync);
    }

    [RelayCommand]
    private void ToggleMonitoring()
    {
        if (IsMonitoring)
        {
            StopMonitoring();
        }
        else
        {
            StartMonitoring();
        }
    }

    public async Task InitializeAsync()
    {
        UserPhone = await SecureStorage.GetAsync(Constants.UserPhoneKey);
        AreaName = await SecureStorage.GetAsync(Constants.AreaNameKey);
        SmtpUser = await SecureStorage.GetAsync(Constants.SmtpUserKey);
        SmtpPassword = await SecureStorage.GetAsync(Constants.SmtpPasswordKey);
    }

    public async Task SaveEmailSettingsAsync(string email, string password)
    {
        SmtpUser = email;
        SmtpPassword = password;
        await SecureStorage.SetAsync(Constants.SmtpUserKey, email);
        await SecureStorage.SetAsync(Constants.SmtpPasswordKey, password);
    }

    [RelayCommand]
    private async void StartMonitoring()
    {
        if (IsMonitoring) return;

        IsMonitoring = true;
        ApiStatusText = "监控中";
        
        DeviceDisplay.KeepScreenOn = true;

        var emailResult = await _emailService.SendEmailAsync("Parking Monitor Started", 
            $"监控已启动\n用户: {UserPhone}\n区域: {AreaName}\n时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
         
        await ShowToastAsync(emailResult.Message);

        _timer.Start();
        _countdownTimer.Start();

        await CheckOrder();
    }

    [RelayCommand]
    private void StopMonitoring()
    {
        if (!IsMonitoring) return;

        IsMonitoring = false;
        ApiStatusText = "已停止";
        DeviceDisplay.KeepScreenOn = false;

        _timer.Stop();
        _countdownTimer.Stop();
    }

    [RelayCommand]
    private async Task CheckOrder()
    {
        ApiStatusText = "查询中...";

        var response = await _apiService.GetLatestOrderAsync();

        if (response.Status == 401)
        {
            ApiStatusText = "Token 已过期";
            var emailResult = await _emailService.SendEmailAsync("Parking Monitor Alert", "Your token has expired. Please login again.");
            if (!emailResult.Success)
            {
                await ShowToastAsync(emailResult.Message);
            }
            StopMonitoring();
            return;
        }

        if (response.Status == 1 && response.Data != null)
        {
            LatestOrder = response.Data;
            HasOrder = true;
            ApiStatusText = "OK";
            LastCheckTime = DateTime.Now.ToString("HH:mm:ss");
            NextCheckTime = DateTime.Now.AddMinutes(10).ToString("HH:mm:ss");
            HasTimeInfo = true;

            _countdownSeconds = 600;

            if (LatestOrder.State != 3)
            {
                if (_lastOrderState == 3 || _lastOrderState == -1)
                {
                    var emailResult = await _emailService.SendEmailAsync("New Parking Order", 
                        $"Car: {LatestOrder.Car}\nPark: {LatestOrder.ParkName}\nTime: {LatestOrder.StartTime}");
                    if (!emailResult.Success)
                    {
                        await ShowToastAsync(emailResult.Message);
                    }
                }
            }
            _lastOrderState = LatestOrder.State;
        }
        else
        {
            ApiStatusText = "Error: " + response.Message;
        }
    }

    private void UpdateCountdown()
    {
        if (_countdownSeconds > 0)
        {
            _countdownSeconds--;
            CountdownText = $"{_countdownSeconds}s";
        }
    }

    private async Task ShowEmailSettingsAsync()
    {
        if (Shell.Current.CurrentPage is MainPage mainPage)
        {
            await mainPage.ShowEmailSettingsPopupAsync();
        }
    }

    private async Task ShowToastAsync(string message)
    {
        if (Shell.Current.CurrentPage is Page page)
        {
            await page.DisplayAlertAsync("提示", message, "确定");
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        StopMonitoring();
        SecureStorage.Remove(Constants.AuthTokenKey);
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
