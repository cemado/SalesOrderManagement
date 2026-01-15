using System;
using System.Collections.Generic;
using System.Linq;

namespace WcfService
{
    /// <summary>
    /// Implementación del servicio WCF
    /// Simula el comportamiento de un servicio legacy
    /// </summary>
    public class OrdenService : IOrdenService
    {
        // Almacenamiento simulado en memoria  
        private static readonly Dictionary<int, OrdenWcfDto> _ordenes = new();
        private static int _nextId = 1;
        private static readonly object _lockObject = new();

        public OrdenService()
        {
            Console.WriteLine("[WCF SERVICE] Instancia de servicio creada");
        }

        public int RegistrarOrden(OrdenWcfDto orden)
        {
            try
            {
                lock (_lockObject)
                {
                    Console.WriteLine($"[WCF] ➤ Registrando orden para cliente: {orden.Cliente}");

                    if (string.IsNullOrWhiteSpace(orden.Cliente))
                        throw new OrdenFault("Cliente es requerido", 400);

                    if (!orden.Detalles.Any())
                        throw new OrdenFault("Debe tener al menos un detalle", 400);

                    if (orden.Detalles.Any(d => d.Cantidad <= 0 || d.PrecioUnitario < 0))
                        throw new OrdenFault("Cantidad debe ser positiva y precio no puede ser negativo", 400);

                    var ordenDuplicada = _ordenes.Values.FirstOrDefault(o =>
                        o.Cliente == orden.Cliente &&
                        o.Fecha.Date == orden.Fecha.Date);

                    if (ordenDuplicada != null)
                        throw new OrdenFault($"Ya existe una orden para {orden.Cliente} en {orden.Fecha.Date:yyyy-MM-dd}", 409);

                    orden.Id = _nextId++;
                    orden.Total = orden.Detalles.Sum(d => d.Subtotal);
                    orden.Estado = "Pendiente";

                    _ordenes[orden.Id] = orden;

                    Console.WriteLine($"[WCF] ✓ Orden #{orden.Id} registrada exitosamente (Total: ${orden.Total})");
                    return orden.Id;
                }
            }
            catch (OrdenFault)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WCF] ✗ Error inesperado: {ex.Message}");
                throw new OrdenFault($"Error al registrar orden: {ex.Message}", 500);
            }
        }

        public OrdenWcfDto ObtenerOrden(int id)
        {
            lock (_lockObject)
            {
                if (_ordenes.TryGetValue(id, out var orden))
                {
                    Console.WriteLine($"[WCF] ✓ Obteniendo orden #{id}");
                    return orden;
                }

                throw new OrdenFault($"Orden {id} no encontrada", 404);
            }
        }

        public List<OrdenWcfDto> ListarOrdenes()
        {
            lock (_lockObject)
            {
                Console.WriteLine($"[WCF] ✓ Listando {_ordenes.Count} orden(es)");
                return _ordenes.Values.ToList();
            }
        }

        public bool ActualizarOrden(OrdenWcfDto orden)
        {
            lock (_lockObject)
            {
                if (!_ordenes.ContainsKey(orden.Id))
                    throw new OrdenFault($"Orden {orden.Id} no encontrada", 404);

                orden.Total = orden.Detalles.Sum(d => d.Subtotal);
                _ordenes[orden.Id] = orden;
                Console.WriteLine($"[WCF] ✓ Orden #{orden.Id} actualizada");
                return true;
            }
        }

        public bool EliminarOrden(int id)
        {
            lock (_lockObject)
            {
                if (_ordenes.Remove(id))
                {
                    Console.WriteLine($"[WCF] ✓ Orden #{id} eliminada");
                    return true;
                }

                throw new OrdenFault($"Orden {id} no encontrada", 404);
            }
        }
    }
}
