using MediatR;
using Application.DTOs;

namespace Application.CQRS.Commands
{
    /// <summary>
    /// Comando para crear una nueva orden
    /// </summary>
    public class CrearOrdenCommand : IRequest<OrdenResponse>
    {
        public CrearOrdenRequest Orden { get; set; } = new();
    }
}
