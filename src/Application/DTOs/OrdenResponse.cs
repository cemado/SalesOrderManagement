namespace Application.DTOs
{
    /// <summary>
    /// Response para consultas de orden
    /// </summary>
    public class OrdenResponse
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public List<DetalleOrdenDto> Detalles { get; set; } = new();
    }
}
