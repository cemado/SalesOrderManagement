namespace Frontend.Models
{
    /// <summary>
    /// Modelo para mostrar órdenes en las vistas
    /// </summary>
    public class OrdenViewModel
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public List<DetalleOrdenViewModel> Detalles { get; set; } = new();
    }

    /// <summary>
    /// Modelo para detalles de una orden
    /// </summary>
    public class DetalleOrdenViewModel
    {
        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    /// <summary>
    /// Respuesta paginada de la API
    /// </summary>
    public class PaginatedResponse
    {
        public List<OrdenViewModel> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}