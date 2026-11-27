using MiApp.Views;
using System.Xml;
using Tarea_8.Services;

namespace Tarea_8.Views;

public partial class ProfilePage : ContentPage
{
    private readonly AuthService _authService;

    public ProfilePage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadProfileAsync();
    }

    private async Task LoadProfileAsync()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        ProfileContainer.IsVisible = false;
        ErrorLabel.IsVisible = false;

        var (profile, error) = await _authService.GetProfileAsync();

        LoadingIndicator.IsRunning = false;
        LoadingIndicator.IsVisible = false;

        if (profile is null)
        {
            ErrorLabel.Text = error ?? "No se pudo cargar el perfil.";
            ErrorLabel.IsVisible = true;

            // Si no hay token, regresar al login
            if (!await _authService.HasValidSessionAsync())
                await Shell.Current.GoToAsync("//login");
            return;
        }

        NameLabel.Text = $"{profile.FirstName} {profile.LastName}".Trim();
        EmailLabel.Text = profile.Email;
        ProfileImage.Source = profile.Image;
        ProfileContainer.IsVisible = true;
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Cerrar sesión",
            "¿Deseas salir de tu cuenta?", "Sí", "No");
        if (!confirm) return;

        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//login");
    }
}