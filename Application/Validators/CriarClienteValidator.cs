using FluentValidation;
using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Application.Validators;

public class CriarClienteValidator : AbstractValidator<Cliente>
{
    public CriarClienteValidator()
    {
        RuleFor(x => x.NomeCompleto).NotEmpty().MaximumLength(200);
    }
}
