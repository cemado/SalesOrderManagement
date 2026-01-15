namespace Domain.Entities
{
    /// <summary>
    /// Entidad que representa una Orden de Venta
    /// </summary>
    public class Orden
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";

        // Navegación
        public ICollection<DetalleOrden> Detalles { get; set; } = new List<DetalleOrden>();

        /// <summary>
        /// Calcula el total de la orden basado en sus detalles
        /// </summary>
        public void CalcularTotal()
        {
            Total = Detalles.Sum(d => d.Subtotal);
        }

        /// <summary>
        /// Valida que la orden sea válida según las reglas de negocio
        /// </summary>
        public bool EsValida()
        {
            return !string.IsNullOrWhiteSpace(Cliente) &&
                   Detalles.Any() &&
                   Detalles.All(d => d.Cantidad > 0 && d.PrecioUnitario >= 0) &&
                   Total >= 0;
        }
    }
}
