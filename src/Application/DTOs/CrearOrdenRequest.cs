namespace Application.DTOs
{
    /// <summary>
    /// Request para crear una nueva orden
    /// </summary>
    public class CrearOrdenRequest
    {
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public List<CrearDetalleRequest> Detalles { get; set; } = new();
    }

    /// <summary>
    /// Request para crear un detalle de orden
    /// </summary>
    public class CrearDetalleRequest
    {
        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
