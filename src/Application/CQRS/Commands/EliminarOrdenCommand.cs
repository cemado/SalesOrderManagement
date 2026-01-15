using MediatR;

namespace Application.CQRS.Commands
{
    /// <summary>
    /// Comando para eliminar una orden
    /// </summary>
    public class EliminarOrdenCommand : IRequest<bool>
    {
        public int OrdenId { get; set; }
    }
}
