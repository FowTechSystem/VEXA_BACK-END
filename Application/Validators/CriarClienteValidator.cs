using FluentValidation;
using CRMVEXA.Application.Commands.Clientes;

namespace CRMVEXA.Application.Validators
{
    public class CriarClienteValidator : AbstractValidator<CriarClienteCommand>
    {
        public CriarClienteValidator()
        {
            RuleFor(x => x.NomeCompleto).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Telefone).NotEmpty().MaximumLength(20);
            RuleFor(x => x.WhatsApp).NotEmpty().MaximumLength(20);
            RuleFor(x => x.CEP).Length(8);
            RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}
