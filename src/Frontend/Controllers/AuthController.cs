using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using Frontend.Services;

namespace Frontend.Controllers
{
    /// <summary>
    /// Controlador de autenticación
    /// </summary>
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// GET: /Auth/Login
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir
            if (HttpContext.Session.GetString("JwtToken") != null)
            {
                return RedirectToAction("Index", "Ordenes");
            }

            return View(new LoginViewModel());
        }

        /// <summary>
        /// POST: /Auth/Login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Por favor, complete todos los campos";
                return View(model);
            }

            try
            {
                Console.WriteLine($"🔐 Intentando login: {model.Username}");
                
                var response = await _apiService.LoginAsync(model.Username, model.Password);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    // ===== DEBUG: VERIFICAR QUE EL ROL NO ESTÉ VACÍO =====
                    Console.WriteLine($"📥 Respuesta recibida:");
                    Console.WriteLine($"   - Token: {response.Token.Substring(0, 20)}...");
                    Console.WriteLine($"   - Rol: '{response.Rol}' (Length: {response.Rol?.Length ?? 0})");

                    if (string.IsNullOrWhiteSpace(response.Rol))
                    {
                        Console.WriteLine("⚠️ ADVERTENCIA: El rol está vacío!");
                        model.ErrorMessage = "Error: La API no devolvió el rol del usuario. Contacte al administrador.";
                        return View(model);
                    }

                    // ===== GUARDAR EN SESIÓN =====
                    HttpContext.Session.SetString("JwtToken", response.Token);
                    HttpContext.Session.SetString("UserRole", response.Rol);
                    HttpContext.Session.SetString("Username", model.Username);

                    // DEBUG: Verificar que se guardó correctamente
                    var tokenGuardado = HttpContext.Session.GetString("JwtToken");
                    var roleGuardado = HttpContext.Session.GetString("UserRole");
                    var usernameGuardado = HttpContext.Session.GetString("Username");

                    Console.WriteLine($"💾 Guardado en sesión:");
                    Console.WriteLine($"   - Token guardado: {tokenGuardado?.Substring(0, 20)}...");
                    Console.WriteLine($"   - Rol guardado: '{roleGuardado}'");
                    Console.WriteLine($"   - Username guardado: '{usernameGuardado}'");

                    if (string.IsNullOrWhiteSpace(roleGuardado))
                    {
                        Console.WriteLine("❌ ERROR CRÍTICO: El rol NO se guardó en la sesión!");
                    }
                    else
                    {
                        Console.WriteLine($"✓ Login exitoso: {model.Username} | Rol: {roleGuardado}");
                    }

                    TempData["Success"] = $"Bienvenido, {model.Username}!";
                    return RedirectToAction("Index", "Ordenes");
                }

                model.ErrorMessage = "Usuario o contraseña incorrectos. Por favor, intente nuevamente.";
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en Login: {ex.Message}");
                Console.WriteLine($"   Stack: {ex.StackTrace}");
                model.ErrorMessage = $"Error al iniciar sesión: {ex.Message}";
                return View(model);
            }
        }

        /// <summary>
        /// GET: /Auth/Logout
        /// </summary>
        public IActionResult Logout()
        {
            Console.WriteLine($"🚪 Logout de usuario: {HttpContext.Session.GetString("Username")}");
            HttpContext.Session.Clear();
            TempData["Success"] = "Ha cerrado sesión exitosamente";
            return RedirectToAction("Login");
        }
    }
}