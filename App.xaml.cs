using Microsoft.Extensions.DependencyInjection;

namespace FreeParkingMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // Check if user is logged in
            var token = await SecureStorage.GetAsync(Constants.AuthTokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}
