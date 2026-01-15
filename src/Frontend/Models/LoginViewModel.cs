namespace Frontend.Models
{
    /// <summary>
    /// Modelo para el formulario de inicio de sesión
    /// </summary>
    public class LoginViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Respuesta del endpoint de autenticación JWT
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}