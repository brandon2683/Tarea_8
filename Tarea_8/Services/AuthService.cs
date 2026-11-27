using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Tarea_8.Models;

namespace Tarea_8.Services
{
    public class AuthService
    {
        private const string BaseUrl = "https://dummyjson.com";
        private const string TokenKey = "auth_token";
        private const string UsernameKey = "username";

        private readonly HttpClient _httpClient;

        public AuthService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public async Task<(bool Success, string? Error)> LoginAsync(string username, string password)
        {
            try
            {
                var request = new LoginRequest { Username = username, Password = password };
                var response = await _httpClient.PostAsJsonAsync("/auth/login", request);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                    return (false, "Usuario o contraseña incorrectos.");

                if (!response.IsSuccessStatusCode)
                    return (false, $"Error del servidor ({(int)response.StatusCode}).");

                var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (data is null || string.IsNullOrEmpty(data.AccessToken))
                    return (false, "Respuesta inválida del servidor.");


                await SecureStorage.Default.SetAsync(TokenKey, data.AccessToken);
                Preferences.Default.Set(UsernameKey, username);

                return (true, null);
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar. Verifica tu conexión a internet.");
            }
            catch (TaskCanceledException)
            {
                return (false, "La solicitud tardó demasiado. Intenta de nuevo.");
            }
            catch (Exception ex)
            {
                return (false, $"Error inesperado: {ex.Message}");
            }
        }

        public async Task<(UserProfile? Profile, string? Error)> GetProfileAsync()
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync(TokenKey);
                if (string.IsNullOrEmpty(token))
                    return (null, "No hay sesión activa.");

                using var request = new HttpRequestMessage(HttpMethod.Get, "/auth/me");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await LogoutAsync();
                    return (null, "Sesión expirada. Inicia sesión de nuevo.");
                }

                if (!response.IsSuccessStatusCode)
                    return (null, $"Error al obtener perfil ({(int)response.StatusCode}).");

                var profile = await response.Content.ReadFromJsonAsync<UserProfile>();
                return (profile, null);
            }
            catch (HttpRequestException)
            {
                return (null, "No se pudo conectar. Verifica tu conexión.");
            }
            catch (Exception ex)
            {
                return (null, $"Error inesperado: {ex.Message}");
            }
        }

        public Task LogoutAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
            Preferences.Default.Remove(UsernameKey);
            return Task.CompletedTask;
        }

        public async Task<bool> HasValidSessionAsync()
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);
            return !string.IsNullOrEmpty(token);
        }

        public string GetSavedUsername() =>
            Preferences.Default.Get(UsernameKey, string.Empty);
    }
}
