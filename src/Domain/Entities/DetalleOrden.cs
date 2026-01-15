namespace Domain.Entities
{
    /// <summary>
    /// Entidad que representa un Detalle de Orden de Venta
    /// </summary>
    public class DetalleOrden
    {
        public int Id { get; set; }
        public int OrdenId { get; set; }
        public string Producto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal 
        { 
            get => Cantidad * PrecioUnitario; 
        }

        // Navegación
        public Orden? Orden { get; set; }
    }
}
