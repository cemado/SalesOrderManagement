using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controlador de autenticación - Genera tokens JWT
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Endpoint de login - Genera token JWT válido por 24 horas
        /// </summary>
        /// <remarks>
        /// **Usuarios de prueba disponibles:**
        /// 
        /// **1. Admin (acceso total)**
        /// - Usuario: `admin@test.com`
        /// - Contraseña: `admin123`
        /// - Permisos: CRUD completo (Create, Read, Update, Delete)
        /// 
        /// **2. Vendedor (lectura y creación)**
        /// - Usuario: `vendedor@test.com`
        /// - Contraseña: `vendedor123`
        /// - Permisos: Solo lectura y creación de órdenes
        /// </remarks>
        /// <param name="request">Credenciales del usuario (username y password)</param>
        /// <returns>Token JWT y rol del usuario</returns>
        /// <response code="200">Login exitoso - retorna token JWT y rol</response>
        /// <response code="401">Credenciales inválidas</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { mensaje = "Usuario y contraseña son requeridos" });

            // Validar credenciales
            var usuario = ValidarUsuario(request.Username, request.Password);
            if (usuario == null)
                return Unauthorized(new { mensaje = "Credenciales inválidas" });

            // Generar token
            var token = GenerarToken(usuario.Username, usuario.Rol);

            // ⚠️ CORRECCIÓN CRÍTICA: Devolver token Y rol
            return Ok(new 
            { 
                token = token,
                rol = usuario.Rol  // ← AGREGAR ESTA LÍNEA
            });
        }

        /// <summary>
        /// Valida las credenciales del usuario contra la lista de usuarios permitidos
        /// </summary>
        private Usuario? ValidarUsuario(string username, string password)
        {
            // Usuarios hardcoded para demostración
            // En producción, estos deberían estar en BD con hash de contraseña
            if (username == "admin@test.com" && password == "admin123")
                return new Usuario { Username = "admin@test.com", Rol = "Admin" };

            if (username == "vendedor@test.com" && password == "vendedor123")
                return new Usuario { Username = "vendedor@test.com", Rol = "Vendedor" };

            return null;
        }

        /// <summary>
        /// Genera un token JWT válido
        /// </summary>
        private string GenerarToken(string username, string rol)
        {
            var secretKey = _configuration["Jwt:SecretKey"] 
                ?? "tu-clave-secreta-super-mega-larga-para-produccion-minimo-32-caracteres";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Email, username),
                new Claim(ClaimTypes.Role, rol)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "SalesOrderAPI",
                audience: _configuration["Jwt:Audience"] ?? "SalesOrderAPIUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Modelo de solicitud para login
        /// </summary>
        public class LoginRequest
        {
            /// <summary>
            /// Usuario/email
            /// </summary>
            /// <example>admin@test.com</example>
            public string Username { get; set; } = string.Empty;

            /// <summary>
            /// Contraseña
            /// </summary>
            /// <example>admin123</example>
            public string Password { get; set; } = string.Empty;
        }

        /// <summary>
        /// Modelo interno de usuario
        /// </summary>
        private class Usuario
        {
            public string Username { get; set; } = string.Empty;
            public string Rol { get; set; } = string.Empty;
        }
    }
}
