using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Application.Mappings
{
    public class CaixaProfile : Profile
    {
        public CaixaProfile()
        {
            CreateMap<Caixa, CaixaDto>();
        }
    }
}
