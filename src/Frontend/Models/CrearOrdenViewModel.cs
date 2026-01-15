using System.ComponentModel.DataAnnotations;

namespace Frontend.Models
{
    /// <summary>
    /// Modelo para crear y editar órdenes
    /// </summary>
    public class CrearOrdenViewModel
    {
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El cliente es obligatorio")]
        [StringLength(200, ErrorMessage = "El nombre del cliente no puede exceder 200 caracteres")]
        public string Cliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe agregar al menos un detalle")]
        [MinLength(1, ErrorMessage = "Debe agregar al menos un producto")]
        public List<DetalleOrdenViewModel> Detalles { get; set; } = new();
    }
}