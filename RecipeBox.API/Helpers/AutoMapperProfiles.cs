using System.Linq;
using AutoMapper;
using RecipeBox.API.Dtos;
using RecipeBox.API.Models;

namespace RecipeBox.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember( dest => dest.PhotoUrl, opt =>
                    opt.MapFrom(src => src.UserPhotos.FirstOrDefault(p => p.IsMain).Url));
            CreateMap<User, UserForDetailedDto>()
                .ForMember( dest => dest.PhotoUrl, opt =>
                    opt.MapFrom(src => src.UserPhotos.FirstOrDefault(p => p.IsMain).Url));
            CreateMap<UserPhoto, PhotosForDetailedDto>();
            CreateMap<Post, PostsForDetailedDto>();
            CreateMap<Post, PostsForListDto>();
            CreateMap<PostForCreationDto, Post>();
            // CreateMap<Post, PostForCreationDto>();
            CreateMap<Post, PostForUpdateDto>();
            CreateMap<PostForUpdateDto, Post>();
            // CreateMap<CommentsForReturnedDto, Comment>();
            CreateMap<CommentForCreationDto, Comment>();
            CreateMap<Comment, CommentsForReturnedDto>();
            CreateMap<Comment, CommentForUpdateDto>();
            CreateMap<CommentForUpdateDto, Comment>();
            CreateMap<Comment, PostsForDetailedDto>();
        
        }
    }
}