using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Frontend.Models;

namespace Frontend.Services
{
    /// <summary>
    /// Servicio para consumir la API REST con autenticación JWT
    /// </summary>
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001";
        }

        /// <summary>
        /// Agrega el token JWT al header
        /// </summary>
        private void SetAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// Login - Autentica usuario y obtiene token JWT
        /// </summary>
        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            var loginRequest = new { username, password };
            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    
                    // DEBUG: Imprimir respuesta cruda
                    Console.WriteLine($"🔍 Respuesta API Login: {responseBody}");
                    
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // DEBUG: Verificar deserialización
                    if (loginResponse != null)
                    {
                        Console.WriteLine($"✓ Token recibido: {loginResponse.Token.Substring(0, 20)}...");
                        Console.WriteLine($"✓ Rol recibido: '{loginResponse.Rol}'");
                    }

                    return loginResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en LoginAsync: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Obtiene todas las órdenes con paginación
        /// </summary>
        public async Task<PaginatedResponse?> GetOrdenesAsync(int pageNumber = 1, int pageSize = 10)
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/ordenes?pageNumber={pageNumber}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PaginatedResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetOrdenesAsync: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Obtiene una orden específica por ID
        /// </summary>
        public async Task<OrdenViewModel?> GetOrdenByIdAsync(int id)
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/ordenes/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<OrdenViewModel>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetOrdenByIdAsync: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Crea una nueva orden
        /// </summary>
        public async Task<bool> CreateOrdenAsync(CrearOrdenViewModel model)
        {
            SetAuthorizationHeader();

            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/ordenes", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateOrdenAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Actualiza una orden existente (cabecera + detalles)
        /// </summary>
        public async Task<bool> UpdateOrdenAsync(int id, CrearOrdenViewModel model)
        {
            SetAuthorizationHeader();

            try
            {
                // ===== MAPEAR A REQUEST =====
                var request = new
                {
                    fecha = model.Fecha,
                    cliente = model.Cliente,
                    detalles = model.Detalles.Select(d => new
                    {
                        id = (int?)null, // Los detalles se reemplazan completamente
                        producto = d.Producto,
                        cantidad = d.Cantidad,
                        precioUnitario = d.PrecioUnitario
                    }).ToList()
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"🔄 Actualizando orden #{id}:");
                Console.WriteLine($"   Cliente: {model.Cliente}");
                Console.WriteLine($"   Detalles: {model.Detalles.Count}");

                var response = await _httpClient.PutAsync($"{_baseUrl}/api/ordenes/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✓ Orden #{id} actualizada exitosamente");
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error: {response.StatusCode} - {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en UpdateOrdenAsync: {ex.Message}");
                Console.WriteLine($"   Stack: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Elimina una orden
        /// </summary>
        public async Task<bool> DeleteOrdenAsync(int id)
        {
            SetAuthorizationHeader();

            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/ordenes/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteOrdenAsync: {ex.Message}");
                return false;
            }
        }
    }
}