using FreeParkingMaui.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace FreeParkingMaui.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;
    private Grid? _overlay;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }

    public async Task ShowEmailSettingsPopupAsync()
    {
        var emailEntry = new Entry
        {
            Placeholder = "QQ邮箱地址",
            Text = _viewModel.SmtpUser ?? "",
            Keyboard = Keyboard.Email
        };

        var passwordEntry = new Entry
        {
            Placeholder = "SMTP授权码",
            Text = _viewModel.SmtpPassword ?? ""
        };

        var tcs = new TaskCompletionSource<bool>();

        var saveButton = new Button
        {
            Text = "保存",
            BackgroundColor = Color.FromArgb("#007AFF"),
            TextColor = Colors.White,
            CornerRadius = 8,
            HeightRequest = 40
        };
        saveButton.Clicked += (s, e) => tcs.TrySetResult(true);

        var cancelButton = new Button
        {
            Text = "取消",
            BackgroundColor = Colors.White,
            TextColor = Color.FromArgb("#666"),
            BorderColor = Color.FromArgb("#DDD"),
            BorderWidth = 1,
            CornerRadius = 8,
            HeightRequest = 40
        };
        cancelButton.Clicked += (s, e) => tcs.TrySetResult(false);

        var buttonGrid = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition() },
            ColumnSpacing = 10,
            Margin = new Thickness(0, 10, 0, 0)
        };
        buttonGrid.Add(saveButton, 0, 0);
        buttonGrid.Add(cancelButton, 1, 0);

        var popupContent = new Border
        {
            BackgroundColor = Colors.White,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 20,
            Stroke = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 300,
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Children =
                {
                    new Label
                    {
                        Text = "邮箱设置",
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label { Text = "QQ邮箱:", FontSize = 13, TextColor = Color.FromArgb("#888") },
                    emailEntry,
                    new Label { Text = "SMTP授权码:", FontSize = 13, TextColor = Color.FromArgb("#888") },
                    passwordEntry,
                    buttonGrid
                }
            }
        };

        _overlay = new Grid
        {
            BackgroundColor = Color.FromArgb("#80000000"),
            Children = { popupContent }
        };

        if (Content is Grid mainGrid)
        {
            mainGrid.Children.Add(_overlay);
        }

        var result = await tcs.Task;

        if (Content is Grid mg && _overlay != null)
        {
            mg.Children.Remove(_overlay);
        }

        if (result)
        {
            await _viewModel.SaveEmailSettingsAsync(emailEntry.Text ?? "", passwordEntry.Text ?? "");
            await DisplayAlertAsync("成功", "邮箱设置已保存", "确定");
        }
    }
}
