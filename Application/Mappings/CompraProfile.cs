using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Application.Mappings
{
    public class CompraProfile : Profile
    {
        public CompraProfile()
        {
            CreateMap<Compra, CompraDto>();
            CreateMap<ItemCompra, ItemCompraDto>();
        }
    }
}
