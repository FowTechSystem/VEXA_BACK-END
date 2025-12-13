//using FluentValidation;
//using CRMVEXA.Application.Commands.Vendas;

//namespace CRMVEXA.Application.Validators
//{
//    public class CriarVendaValidator : AbstractValidator<CriarVendaCommand>
//    {
//        public CriarVendaValidator()
//        {
//            RuleFor(x => x.CodigoVenda).NotEmpty().MaximumLength(50);
//            RuleFor(x => x.FormaPagamento).NotEmpty();
//            RuleFor(x => x.UsuarioId).GreaterThan(0);
//            RuleFor(x => x.Itens).NotEmpty();
//        }
//    }
//}
