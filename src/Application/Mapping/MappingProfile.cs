using AutoMapper;
using Domain.Entities;
using Application.DTOs;

namespace Application.Mapping
{
    /// <summary>
    /// Perfil de mapeos entre entidades del dominio y DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Orden
            CreateMap<Orden, OrdenDto>().ReverseMap();
            CreateMap<Orden, OrdenResponse>().ReverseMap();

            // DetalleOrden
            CreateMap<DetalleOrden, DetalleOrdenDto>().ReverseMap();

            // Requests
            CreateMap<CrearOrdenRequest, Orden>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => "Pendiente"))
                .ForMember(dest => dest.Total, opt => opt.Ignore());
        }
    }
}
