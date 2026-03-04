using Microsoft.Extensions.Logging;
using FreeParkingMaui.Services;
using FreeParkingMaui.ViewModels;
using FreeParkingMaui.Views;

namespace FreeParkingMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Services
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<EmailService>();
            builder.Services.AddSingleton<AudioService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MainViewModel>();

            // Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
