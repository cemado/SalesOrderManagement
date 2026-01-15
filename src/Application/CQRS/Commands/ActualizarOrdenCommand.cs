using MediatR;

namespace Application.CQRS.Commands
{
    /// <summary>
    /// Comando para actualizar una orden completa (cabecera + detalles)
    /// </summary>
    public class ActualizarOrdenCommand : IRequest<bool>
    {
        public int OrdenId { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public List<ActualizarDetalleCommand> Detalles { get; set; } = new();
    }

    /// <summary>
    /// Comando para actualizar un detalle de orden
    /// </summary>
    public class ActualizarDetalleCommand
    {
        public int? Id { get; set; } // null = nuevo detalle
        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
