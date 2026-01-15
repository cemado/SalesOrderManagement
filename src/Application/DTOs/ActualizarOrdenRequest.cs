namespace Application.DTOs
{
    /// <summary>
    /// Request para actualizar una orden completa
    /// </summary>
    public class ActualizarOrdenRequest
    {
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public List<ActualizarDetalleRequest> Detalles { get; set; } = new();
    }

    /// <summary>
    /// Request para actualizar un detalle de orden
    /// </summary>
    public class ActualizarDetalleRequest
    {
        public int? Id { get; set; } // null = nuevo detalle
        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}