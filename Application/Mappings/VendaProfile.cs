using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRMVEXA.Application.Mappings
{
    public class VendaProfile : Profile
    {
        public VendaProfile()
        {
            CreateMap<Venda, VendaDto>();
            CreateMap<ItemVenda, ItemVendaDto>();
        }
    }
}
