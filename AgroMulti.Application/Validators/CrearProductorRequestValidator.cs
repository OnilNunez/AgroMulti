using AgroMulti.Domain.Requests;
using FluentValidation;

namespace AgroMulti.Application.Validators;

public class CrearProductorRequestValidator
    : AbstractValidator<CrearProductorRequest>
{
    public CrearProductorRequestValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Apellido)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Telefono)
            .MaximumLength(20);

        RuleFor(x => x.Direccion)
            .MaximumLength(200);
    }
}