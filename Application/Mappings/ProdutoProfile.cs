using System.Linq;
using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Application.Features.Clientes;
using CRMVEXA.Domain.Entities;

namespace CRMVEXA.Application.Mappings
{
    public class ProdutoProfile : Profile
    {
        public ProdutoProfile()
        {
            CreateMap<Produto, ProdutoDto>()
                .ForMember(d => d.Foto1, o => o.MapFrom(s => ClienteFotoUrlHelper.ToApiFotoUrl(s.Foto1)))
                .ForMember(d => d.Foto2, o => o.MapFrom(s => ClienteFotoUrlHelper.ToApiFotoUrl(s.Foto2)))
                .ForMember(d => d.Foto3, o => o.MapFrom(s => ClienteFotoUrlHelper.ToApiFotoUrl(s.Foto3)))
                .ForMember(d => d.Foto4, o => o.MapFrom(s => ClienteFotoUrlHelper.ToApiFotoUrl(s.Foto4)))
                .ForMember(d => d.Foto5, o => o.MapFrom(s => ClienteFotoUrlHelper.ToApiFotoUrl(s.Foto5)))
                .ForMember(d => d.Cores, o => o.MapFrom(s => s.Cores.ToList()))
                .ForMember(d => d.Tamanhos, o => o.MapFrom(s => s.Tamanhos.ToList()));
        }
    }
}
