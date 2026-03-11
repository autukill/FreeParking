using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FreeParkingMaui.Models;
using FreeParkingMaui.Services;

namespace FreeParkingMaui.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly ApiService _apiService;

    [ObservableProperty]
    private string _phoneNumber;

    [ObservableProperty]
    private string _authCode;

    [ObservableProperty]
    private Area _selectedArea;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage;

    public List<Area> Areas => Constants.Areas;

    public LoginViewModel(ApiService apiService)
    {
        _apiService = apiService;
        SelectedArea = Areas.First()!;
    }

    [RelayCommand]
    private async Task GetCode()
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber))
        {
            StatusMessage = "Please enter phone number.";
            return;
        }

        IsBusy = true;
        StatusMessage = "Sending code...";
        
        var response = await _apiService.GetVerificationCodeAsync(PhoneNumber, SelectedArea?.AppId ?? Areas[0].AppId);
        
        IsBusy = false;
        if (response.Status == 1 || response.Message == "OK")
        {
            StatusMessage = "Code sent!";
        }
        else
        {
            StatusMessage = $"Failed: {response.Message}";
        }
    }

    [RelayCommand]
    private async Task Login()
    {
        if (string.IsNullOrWhiteSpace(PhoneNumber) || string.IsNullOrWhiteSpace(AuthCode))
        {
            StatusMessage = "Please fill all fields.";
            return;
        }

        IsBusy = true;
        StatusMessage = "Logging in...";

        var response = await _apiService.LoginAsync(PhoneNumber, AuthCode, SelectedArea?.Id ?? Areas[0].Id);

        IsBusy = false;
        if (response.Status == 1 && response.Data != null && !string.IsNullOrEmpty(response.Data.AccessToken))
        {
            // Save Token
            await SecureStorage.SetAsync(Constants.AuthTokenKey, response.Data.AccessToken);
            await SecureStorage.SetAsync(Constants.UserPhoneKey, PhoneNumber);
            await SecureStorage.SetAsync(Constants.AreaIdKey, SelectedArea!.Id);
            await SecureStorage.SetAsync(Constants.AreaNameKey, SelectedArea.Name);

            StatusMessage = "Login Successful!";
            
            // Navigate to Main Page
            await Shell.Current.GoToAsync("//MainPage");
        }
        else
        {
            StatusMessage = $"Login Failed: {response.Message}";
        }
    }
}
