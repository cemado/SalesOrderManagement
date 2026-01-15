using MediatR;
using Domain.Repositories;
using Domain.Exceptions;
using Domain.Entities;

namespace Application.CQRS.Commands
{
    /// <summary>
    /// Handler para actualizar una orden completa (cabecera + detalles)
    /// </summary>
    public class ActualizarOrdenCommandHandler : IRequestHandler<ActualizarOrdenCommand, bool>
    {
        private readonly IOrdenRepository _repository;

        public ActualizarOrdenCommandHandler(IOrdenRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(ActualizarOrdenCommand request, CancellationToken cancellationToken)
        {
            // ===== VALIDAR QUE LA ORDEN EXISTA =====
            var orden = await _repository.GetByIdAsync(request.OrdenId);

            if (orden == null)
                throw new NotFoundException($"La orden {request.OrdenId} no existe");

            // ===== VALIDAR QUE TENGA AL MENOS UN DETALLE =====
            if (!request.Detalles.Any())
                throw new ValidationException("La orden debe tener al menos un detalle");

            // ===== VALIDAR DATOS DE DETALLES =====
            if (request.Detalles.Any(d => d.Cantidad <= 0 || d.PrecioUnitario < 0))
                throw new ValidationException("Cantidad debe ser positiva y precio no puede ser negativo");

            // ===== ACTUALIZAR CABECERA =====
            orden.Fecha = request.Fecha;
            orden.Cliente = request.Cliente;

            // ===== LIMPIAR DETALLES EXISTENTES =====
            orden.Detalles.Clear();

            // ===== AGREGAR NUEVOS DETALLES =====
            foreach (var detalleCmd in request.Detalles)
            {
                var detalle = new DetalleOrden
                {
                    Producto = detalleCmd.Producto,
                    Cantidad = detalleCmd.Cantidad,
                    PrecioUnitario = detalleCmd.PrecioUnitario
                };

                orden.Detalles.Add(detalle);
            }

            // ===== RECALCULAR TOTAL =====
            orden.CalcularTotal();

            // ===== GUARDAR CAMBIOS =====
            await _repository.UpdateAsync(orden);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
