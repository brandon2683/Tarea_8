using Microsoft.Extensions.DependencyInjection;
using Tarea_8.Services;

namespace Tarea_8
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        private readonly IServiceProvider _services;

        public App(AuthService authService, IServiceProvider services)
        {
            InitializeComponent();
            _authService = authService;
            _services = services;
            MainPage = _services.GetRequiredService<AppShell>();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            if (await _authService.HasValidSessionAsync())
                await Shell.Current.GoToAsync("//profile");
            else
                await Shell.Current.GoToAsync("//login");
        }
    }
}