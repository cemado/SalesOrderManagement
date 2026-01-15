namespace Application.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de Orden
    /// </summary>
    public class OrdenDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public List<DetalleOrdenDto> Detalles { get; set; } = new();
    }
}
