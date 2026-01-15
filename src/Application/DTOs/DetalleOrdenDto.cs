namespace Application.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de Detalle de Orden
    /// </summary>
    public class DetalleOrdenDto
    {
        public int Id { get; set; }
        public int OrdenId { get; set; }
        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}
