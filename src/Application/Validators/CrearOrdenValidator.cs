using FluentValidation;
using Application.DTOs;

namespace Application.Validators
{
    /// <summary>
    /// Validador para crear una nueva orden
    /// </summary>
    public class CrearOrdenValidator : AbstractValidator<CrearOrdenRequest>
    {
        public CrearOrdenValidator()
        {
            RuleFor(x => x.Cliente)
                .NotEmpty().WithMessage("El cliente es requerido")
                .MinimumLength(3).WithMessage("El cliente debe tener al menos 3 caracteres")
                .MaximumLength(100).WithMessage("El cliente no puede exceder 100 caracteres");

            RuleFor(x => x.Fecha)
                .NotEmpty().WithMessage("La fecha es requerida")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha no puede ser futura");

            RuleFor(x => x.Detalles)
                .NotEmpty().WithMessage("La orden debe tener al menos un detalle")
                .Must(d => d.Count > 0).WithMessage("Debe tener al menos un detalle");

            RuleForEach(x => x.Detalles)
                .SetValidator(new CrearDetalleValidator());
        }
    }

    /// <summary>
    /// Validador para crear un detalle de orden
    /// </summary>
    public class CrearDetalleValidator : AbstractValidator<CrearDetalleRequest>
    {
        public CrearDetalleValidator()
        {
            RuleFor(x => x.Producto)
                .NotEmpty().WithMessage("El producto es requerido")
                .MinimumLength(2).WithMessage("El producto debe tener al menos 2 caracteres")
                .MaximumLength(100).WithMessage("El producto no puede exceder 100 caracteres");

            RuleFor(x => x.Cantidad)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor que 0")
                .LessThanOrEqualTo(9999).WithMessage("La cantidad no puede exceder 9999");

            RuleFor(x => x.PrecioUnitario)
                .GreaterThanOrEqualTo(0).WithMessage("El precio no puede ser negativo")
                .LessThanOrEqualTo(999999.99m).WithMessage("El precio es demasiado alto");
        }
    }
}
