using MediatR;
using Application.DTOs;
using AutoMapper;
using Domain.Repositories;
using Domain.Exceptions;
using Domain.Entities;

namespace Application.CQRS.Commands
{
    /// <summary>
    /// Handler para crear una nueva orden
    /// </summary>
    public class CrearOrdenCommandHandler : IRequestHandler<CrearOrdenCommand, OrdenResponse>
    {
        private readonly IOrdenRepository _repository;
        private readonly IMapper _mapper;

        public CrearOrdenCommandHandler(IOrdenRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<OrdenResponse> Handle(CrearOrdenCommand request, CancellationToken cancellationToken)
        {
            // Validar que no exista orden duplicada para este cliente en esta fecha
            var existeDuplicada = await _repository.ExisteOrdenEnFechaAsync(
                request.Orden.Cliente,
                request.Orden.Fecha);

            if (existeDuplicada)
            {
                throw new ConflictException(
                    $"Ya existe una orden para {request.Orden.Cliente} en la fecha {request.Orden.Fecha.Date:yyyy-MM-dd}");
            }

            // Crear la orden
            var orden = new Orden
            {
                Fecha = request.Orden.Fecha,
                Cliente = request.Orden.Cliente,
                Estado = "Pendiente"
            };

            // Agregar detalles
            foreach (var detalle in request.Orden.Detalles)
            {
                orden.Detalles.Add(new DetalleOrden
                {
                    Producto = detalle.Producto,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario
                });
            }

            // Calcular total
            orden.CalcularTotal();

            // Guardar en BD
            var ordenGuardada = await _repository.CreateAsync(orden);
            await _repository.SaveChangesAsync();

            // Retornar como DTO
            return _mapper.Map<OrdenResponse>(ordenGuardada);
        }
    }
}
