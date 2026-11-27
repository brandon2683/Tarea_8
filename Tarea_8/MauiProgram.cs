using Microsoft.Extensions.Logging;
using Tarea_8.Services;

namespace Tarea_8
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
                });

            // Servicios
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ProfilePage>();

            return builder.Build();
        }
    }
}
