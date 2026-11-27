using Tarea_8.Services;

namespace Tarea_8.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;
    private bool _isBusy;

    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private void OnFieldChanged(object? sender, TextChangedEventArgs e)
    {
        ErrorLabel.IsVisible = false;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        if (_isBusy) return;

        ErrorLabel.IsVisible = false;
        var user = UsernameEntry.Text?.Trim() ?? string.Empty;
        var pass = PasswordEntry.Text ?? string.Empty;

        // Validación de campos vacíos
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
        {
            ShowError("Usuario y contraseña son obligatorios.");
            return;
        }

        SetBusy(true);
        var (success, error) = await _authService.LoginAsync(user, pass);
        SetBusy(false);

        if (!success)
        {
            ShowError(error ?? "No se pudo iniciar sesión.");
            return;
        }

        PasswordEntry.Text = string.Empty;

        await Shell.Current.GoToAsync("//profile");
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    private void SetBusy(bool busy)
    {
        _isBusy = busy;
        LoadingIndicator.IsRunning = busy;
        LoadingIndicator.IsVisible = busy;
        LoginButton.IsEnabled = !busy;
        UsernameEntry.IsEnabled = !busy;
        PasswordEntry.IsEnabled = !busy;
    }
}