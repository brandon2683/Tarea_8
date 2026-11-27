using System;
using System.Collections.Generic;
using System.Text;

namespace Tarea_8.Models
{
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
}
