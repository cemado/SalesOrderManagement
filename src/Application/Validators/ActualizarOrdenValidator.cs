using FluentValidation;
using Application.DTOs;

namespace Application.Validators
{
    /// <summary>
    /// Validador para actualizar una orden
    /// </summary>
    public class ActualizarOrdenValidator : AbstractValidator<ActualizarOrdenRequest>
    {
        public ActualizarOrdenValidator()
        {
            // ===== VALIDACIONES DE CABECERA =====
            RuleFor(x => x.Fecha)
                .NotEmpty().WithMessage("La fecha es obligatoria");

            RuleFor(x => x.Cliente)
                .NotEmpty().WithMessage("El cliente es obligatorio")
                .MinimumLength(3).WithMessage("El cliente debe tener al menos 3 caracteres")
                .MaximumLength(100).WithMessage("El cliente no puede exceder 100 caracteres");

            // ===== VALIDACIONES DE DETALLES =====
            RuleFor(x => x.Detalles)
                .NotEmpty().WithMessage("La orden debe tener al menos un detalle")
                .Must(d => d.Count > 0).WithMessage("Debe tener al menos un producto");

            // ===== VALIDACIONES POR CADA DETALLE =====
            RuleForEach(x => x.Detalles)
                .ChildRules(detalle =>
                {
                    detalle.RuleFor(d => d.Producto)
                        .NotEmpty().WithMessage("El producto es obligatorio")
                        .MinimumLength(2).WithMessage("El producto debe tener al menos 2 caracteres")
                        .MaximumLength(100).WithMessage("El producto no puede exceder 100 caracteres");

                    detalle.RuleFor(d => d.Cantidad)
                        .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");

                    detalle.RuleFor(d => d.PrecioUnitario)
                        .GreaterThanOrEqualTo(0).WithMessage("El precio no puede ser negativo");
                });
        }
    }
}
