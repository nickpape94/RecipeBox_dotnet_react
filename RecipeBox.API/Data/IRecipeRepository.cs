using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeBox.API.Models;

namespace RecipeBox.API.Data
{
    public interface IRecipeRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        void Update<T>(T entity) where T: class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
        Task<Post> GetPost(int id);
        Task<Comment> GetComment(int commentId);
        Task<IEnumerable<Post>> GetPosts();
    }
}