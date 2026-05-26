using AutoMapper;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserResponseDto>();

        CreateMap<Comment, CommentResponseDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : "User"));

        CreateMap<Artifact, ArtifactResponseDto>()
            .ForMember(dest => dest.ArtistName, opt => opt.MapFrom(src => src.Artist != null ? src.Artist.Name : null))
            .ForMember(dest => dest.Tags, opt => opt.Ignore());

        CreateMap<Artist, ArtistResponseDto>()
            .ForMember(dest => dest.Artifacts, opt => opt.MapFrom(src => src.Artifacts));

        CreateMap<Order, OrderResponseDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : null))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<OrderItem, OrderItemResponseDto>();
    }
}
