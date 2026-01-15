using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using Frontend.Services;

namespace Frontend.Controllers
{
    /// <summary>
    /// Controlador para gestión de órdenes
    /// </summary>
    public class OrdenesController : Controller
    {
        private readonly ApiService _apiService;

        public OrdenesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        /// <summary>
        /// Verifica autenticación
        /// </summary>
        private bool EstaAutenticado()
        {
            return HttpContext.Session.GetString("JwtToken") != null;
        }

        /// <summary>
        /// Obtiene el rol del usuario
        /// </summary>
        private string? ObtenerRol()
        {
            return HttpContext.Session.GetString("UserRole");
        }

        /// <summary>
        /// GET: /Ordenes/Index
        /// </summary>
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            if (!EstaAutenticado())
            {
                Console.WriteLine("⚠️ Usuario no autenticado, redirigiendo a Login");
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var ordenes = await _apiService.GetOrdenesAsync(pageNumber, 10);

                if (ordenes == null)
                {
                    TempData["Error"] = "Error al obtener las órdenes";
                    return View(new PaginatedResponse());
                }

                // ===== PASAR DATOS A LA VISTA =====
                var userRole = ObtenerRol();
                var username = HttpContext.Session.GetString("Username");
                
                ViewBag.Username = username;
                ViewBag.UserRole = userRole;

                // DEBUG MEJORADO
                Console.WriteLine($"📋 Index:");
                Console.WriteLine($"   - Username: '{username}'");
                Console.WriteLine($"   - UserRole: '{userRole}' (Length: {userRole?.Length ?? 0})");
                Console.WriteLine($"   - Es Admin: {userRole == "Admin"}");
                Console.WriteLine($"   - Total órdenes: {ordenes.TotalCount}");

                if (string.IsNullOrWhiteSpace(userRole))
                {
                    Console.WriteLine("❌ PROBLEMA: UserRole está vacío en Index!");
                    TempData["Error"] = "Error: Su sesión no tiene un rol asignado. Por favor, vuelva a iniciar sesión.";
                }

                return View(ordenes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en Index: {ex.Message}");
                TempData["Error"] = $"Error: {ex.Message}";
                return View(new PaginatedResponse());
            }
        }

        /// <summary>
        /// GET: /Ordenes/Details/5
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            if (!EstaAutenticado())
                return RedirectToAction("Login", "Auth");

            try
            {
                var orden = await _apiService.GetOrdenByIdAsync(id);

                if (orden == null)
                {
                    TempData["Error"] = $"Orden #{id} no encontrada";
                    return RedirectToAction("Index");
                }

                ViewBag.Username = HttpContext.Session.GetString("Username");
                ViewBag.UserRole = ObtenerRol();

                return View(orden);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// GET: /Ordenes/Create
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            if (!EstaAutenticado())
                return RedirectToAction("Login", "Auth");

            ViewBag.Username = HttpContext.Session.GetString("Username");
            ViewBag.UserRole = ObtenerRol();

            return View(new CrearOrdenViewModel());
        }

        /// <summary>
        /// POST: /Ordenes/Create
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearOrdenViewModel model)
        {
            if (!EstaAutenticado())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor, complete todos los campos correctamente";
                return View(model);
            }

            try
            {
                var success = await _apiService.CreateOrdenAsync(model);

                if (success)
                {
                    TempData["Success"] = "✓ Orden creada exitosamente";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "Error al crear la orden";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(model);
            }
        }

        /// <summary>
        /// GET: /Ordenes/Edit/5 - ⚠️ SOLO ADMIN
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!EstaAutenticado())
            {
                TempData["Error"] = "Debe iniciar sesión primero";
                return RedirectToAction("Login", "Auth");
            }

            var userRole = ObtenerRol();

            // ===== VALIDACIÓN DE ROL =====
            if (userRole != "Admin")
            {
                TempData["Error"] = "⛔ Solo administradores pueden editar órdenes";
                Console.WriteLine($"⚠️ Intento de edición rechazado: rol={userRole}");
                return RedirectToAction("Index");
            }

            try
            {
                var orden = await _apiService.GetOrdenByIdAsync(id);

                if (orden == null)
                {
                    TempData["Error"] = $"Orden #{id} no encontrada";
                    return RedirectToAction("Index");
                }

                // Convertir a modelo de edición
                var model = new CrearOrdenViewModel
                {
                    Fecha = orden.Fecha,
                    Cliente = orden.Cliente,
                    Detalles = orden.Detalles
                };

                ViewBag.OrdenId = id;
                ViewBag.Username = HttpContext.Session.GetString("Username");
                ViewBag.UserRole = userRole;

                Console.WriteLine($"✓ Edit: Cargando orden #{id} para usuario {ViewBag.Username}");

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// POST: /Ordenes/Edit/5 - ⚠️ SOLO ADMIN
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CrearOrdenViewModel model)
        {
            if (!EstaAutenticado())
                return RedirectToAction("Login", "Auth");

            var userRole = ObtenerRol();

            // ===== VALIDACIÓN DE ROL =====
            if (userRole != "Admin")
            {
                TempData["Error"] = "⛔ Solo administradores pueden editar órdenes";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor, complete todos los campos";
                ViewBag.OrdenId = id;
                return View(model);
            }

            try
            {
                var success = await _apiService.UpdateOrdenAsync(id, model);

                if (success)
                {
                    TempData["Success"] = $"✓ Orden #{id} actualizada exitosamente";
                    return RedirectToAction("Index");
                }

                TempData["Error"] = "Error al actualizar la orden";
                ViewBag.OrdenId = id;
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                ViewBag.OrdenId = id;
                return View(model);
            }
        }

        /// <summary>
        /// POST: /Ordenes/Delete/5 - ⚠️ SOLO ADMIN
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!EstaAutenticado())
            {
                TempData["Error"] = "Debe iniciar sesión primero";
                return RedirectToAction("Login", "Auth");
            }

            var userRole = ObtenerRol();

            // ===== VALIDACIÓN DE ROL =====
            if (userRole != "Admin")
            {
                TempData["Error"] = "⛔ Solo administradores pueden eliminar órdenes";
                Console.WriteLine($"⚠️ Intento de eliminación rechazado: rol={userRole}");
                return RedirectToAction("Index");
            }

            try
            {
                var success = await _apiService.DeleteOrdenAsync(id);

                if (success)
                {
                    TempData["Success"] = $"✓ Orden #{id} eliminada exitosamente";
                }
                else
                {
                    TempData["Error"] = $"No se pudo eliminar la orden #{id}";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}