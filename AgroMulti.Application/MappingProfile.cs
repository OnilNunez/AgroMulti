using AgroMulti.Data.Models;
using AgroMulti.Domain.DTOs;
using AutoMapper;

namespace AgroMulti.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Productor, ProductorDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.ProductorId))
            .ForMember(
                dest => dest.NombreCompleto,
                opt => opt.MapFrom(src => src.Nombre + " " + src.Apellido));

        CreateMap<Producto, ProductoDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.ProductoId));

        CreateMap<SubProducto, SubProductoDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.SubProductoId));

        CreateMap<EstadoEntrega, EstadoEntregaDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.EstadoEntregaId));

        CreateMap<Entrega, EntregaDto>()
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.EntregaId))
            .ForMember(
                dest => dest.FechaEntrega,
                opt => opt.MapFrom(src => src.FechaEntrega))
            .ForMember(
                dest => dest.Productor,
                opt => opt.MapFrom(src => src.Productor.Nombre + " " + src.Productor.Apellido))
            .ForMember(
                dest => dest.Producto,
                opt => opt.MapFrom(src => src.Producto.Nombre))
            .ForMember(
                dest => dest.SubProducto,
                opt => opt.MapFrom(src => src.SubProducto != null ? src.SubProducto.Nombre : null))
            .ForMember(
                dest => dest.Estado,
                opt => opt.MapFrom(src => src.EstadoEntrega.Nombre));

            CreateMap<Usuario, UsuarioDto>()
              .ForMember(
                 dest => dest.Id,
                 opt => opt.MapFrom(src => src.UsuarioId))
              .ForMember(
                 dest => dest.Usuario,
                 opt => opt.MapFrom(src => src.NombreUsuario));
    }

}

