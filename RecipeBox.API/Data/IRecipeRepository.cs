using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeBox.API.Dtos.PostDtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;

namespace RecipeBox.API.Data
{
    public interface IRecipeRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        void Update<T>(T entity) where T: class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(PageParams pageParams);
        Task<User> GetUser(int id);
        Task<User> GetUser(string email);
        Task<Comment> GetComment(int commentId);
        Task<Post> GetPost(int id);
        Task<PagedList<Post>> GetPosts(PageParams pageParams, PostForSearchDto postForSearchDto);
        Task<PagedList<Post>> GetPostsByCuisine(PageParams pageParams, string cuisine);
        Task<PagedList<Post>> SearchPostsByUser(PageParams pageParams, int userId);
        Task<UserPhoto> GetUserPhoto(int photoId);
        Task<UserPhoto> GetMainPhotoForUser(int userId);
        Task<PostPhoto> GetPostPhoto(int photoId);
        Task<PostPhoto> GetMainPhotoForPost(int postId);
        Task<PagedList<Post>> GetFavourites(int userId, PageParams pageParams, PostForSearchDto postForSearchDto);
        Task<Favourite> GetFavourite(int postId, int userId);
        Task<IEnumerable<Rating>> GetRatings(int postId);
        Task<Rating> GetRating(int raterId, int postId);

    }
}