using AutoMapper;
using CRMVEXA.Application.DTOs;
using CRMVEXA.Domain.Entities;
using CRMVEXA.Domain.ValueObjects;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CRMVEXA.Application.Mappings
{
    public class ClienteProfile : Profile
    {
        public ClienteProfile()
        {
            CreateMap<Cliente, ClienteDto>();
            CreateMap<Endereco, EnderecoDto>();
        }
    }
}
