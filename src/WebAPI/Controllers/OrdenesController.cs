using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.CQRS.Commands;
using Application.CQRS.Queries;
using Application.DTOs;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controlador REST para gestionar órdenes de venta
    /// Implementa operaciones CRUD con autenticación JWT
    /// </summary>
    /// <remarks>
    /// **Niveles de Acceso:**
    /// - **Admin**: Acceso total (Crear, Leer, Actualizar, Eliminar)
    /// - **Vendedor**: Solo Crear y Leer
    /// - **Público**: Sin acceso (requiere autenticación)
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdenesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdenesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene todas las órdenes con paginación y filtros opcionales
        /// </summary>
        /// <remarks>
        /// **Roles requeridos:** Admin, Vendedor
        /// 
        /// **Parámetros opcionales:**
        /// - `pageNumber`: Número de página (por defecto 1)
        /// - `pageSize`: Cantidad de registros por página (por defecto 10)
        /// - `clienteFilter`: Filtro por nombre de cliente
        /// - `fechaInicio`: Filtro desde una fecha (formato: yyyy-MM-dd)
        /// - `fechaFin`: Filtro hasta una fecha (formato: yyyy-MM-dd)
        /// 
        /// **Ejemplo:**
        /// ```
        /// GET /api/ordenes?pageNumber=1&pageSize=10&clienteFilter=Acme&fechaInicio=2025-01-01
        /// ```
        /// </remarks>
        /// <param name="pageNumber">Número de página (por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (por defecto 10)</param>
        /// <param name="clienteFilter">Filtro por cliente (opcional)</param>
        /// <param name="fechaInicio">Fecha inicio del rango (opcional)</param>
        /// <param name="fechaFin">Fecha fin del rango (opcional)</param>
        /// <returns>Lista paginada de órdenes</returns>
        /// <response code="200">Órdenes obtenidas exitosamente</response>
        /// <response code="401">No autorizado (token inválido o expirado)</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResponse<OrdenResponse>>> GetOrdenes(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? clienteFilter = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var query = new GetOrdenesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                ClienteFilter = clienteFilter,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene una orden específica por su ID
        /// </summary>
        /// <remarks>
        /// **Roles requeridos:** Admin, Vendedor
        /// 
        /// Retorna la orden con sus detalles asociados.
        /// </remarks>
        /// <param name="id">ID de la orden</param>
        /// <returns>Orden encontrada con detalles</returns>
        /// <response code="200">Orden obtenida exitosamente</response>
        /// <response code="404">Orden no encontrada</response>
        /// <response code="401">No autorizado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrdenResponse>> GetOrden(int id)
        {
            try
            {
                var query = new GetOrdenByIdQuery { OrdenId = id };
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex) when (ex.Message.Contains("no existe"))
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva orden con detalles
        /// </summary>
        /// <remarks>
        /// **Roles requeridos:** Admin, Vendedor
        /// 
        /// **Reglas de negocio:**
        /// - Una orden debe tener al menos un detalle
        /// - No se permite registrar dos órdenes del mismo cliente en la misma fecha
        /// - Cantidad debe ser mayor a 0
        /// - Precio unitario no puede ser negativo
        /// - El total se calcula automáticamente como suma de subtotales
        /// 
        /// **Ejemplo de solicitud:**
        /// ```json
        /// {
        ///   "fecha": "2025-01-15",
        ///   "cliente": "Empresa XYZ",
        ///   "detalles": [
        ///     {
        ///       "producto": "Producto A",
        ///       "cantidad": 10,
        ///       "precioUnitario": 100.50
        ///     },
        ///     {
        ///       "producto": "Producto B",
        ///       "cantidad": 5,
        ///       "precioUnitario": 200.00
        ///     }
        ///   ]
        /// }
        /// ```
        /// </remarks>
        /// <param name="request">Datos de la orden a crear</param>
        /// <returns>Orden creada con su ID</returns>
        /// <response code="201">Orden creada exitosamente</response>
        /// <response code="400">Datos inválidos o validación fallida</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado (rol insuficiente)</response>
        [HttpPost]
        [Authorize(Roles = "Admin,Vendedor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<OrdenResponse>> CreateOrden([FromBody] CrearOrdenRequest request)
        {
            try
            {
                var command = new CrearOrdenCommand { Orden = request };
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetOrden), new { id = result.Id }, result);
            }
            catch (Exception ex) when (ex.Message.Contains("Ya existe"))
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("requerido") || ex.Message.Contains("negativo"))
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una orden existente con todos sus detalles
        /// </summary>
        /// <remarks>
        /// **Roles requeridos:** Solo Admin
        /// 
        /// Actualiza la cabecera y todos los detalles de una orden.
        /// Los detalles existentes se reemplazan completamente.
        /// </remarks>
        /// <param name="id">ID de la orden a actualizar</param>
        /// <param name="request">Datos actualizados (cabecera + detalles)</param>
        /// <returns>Confirmación de actualización</returns>
        /// <response code="200">Orden actualizada exitosamente</response>
        /// <response code="400">Datos inválidos o validación fallida</response>
        /// <response code="404">Orden no encontrada</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado (solo Admin)</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateOrden(int id, [FromBody] ActualizarOrdenRequest request)
        {
            try
            {
                // ===== MAPEAR A COMANDO =====
                var command = new ActualizarOrdenCommand
                {
                    OrdenId = id,
                    Fecha = request.Fecha,
                    Cliente = request.Cliente,
                    Detalles = request.Detalles.Select(d => new ActualizarDetalleCommand
                    {
                        Id = d.Id,
                        Producto = d.Producto,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    }).ToList()
                };

                var result = await _mediator.Send(command);

                return Ok(new
                {
                    mensaje = "Orden actualizada exitosamente",
                    ordenId = id
                });
            }
            catch (Exception ex) when (ex.Message.Contains("no existe"))
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex) when (ex.Message.Contains("Cantidad") || ex.Message.Contains("detalle"))
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Elimina una orden
        /// </summary>
        /// <remarks>
        /// **Roles requeridos:** Solo Admin
        /// 
        /// Solo los administradores pueden eliminar órdenes.
        /// </remarks>
        /// <param name="id">ID de la orden a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Orden eliminada exitosamente</response>
        /// <response code="404">Orden no encontrada</response>
        /// <response code="401">No autorizado</response>
        /// <response code="403">Acceso denegado (solo Admin)</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteOrden(int id)
        {
            try
            {
                var command = new EliminarOrdenCommand { OrdenId = id };
                var result = await _mediator.Send(command);
                return Ok(new { mensaje = "Orden eliminada exitosamente" });
            }
            catch (Exception ex) when (ex.Message.Contains("no existe"))
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }
    }
}
