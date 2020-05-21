using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeBox.API.src.Main.Models;

namespace RecipeBox.API.src.Main.Data
{
    public interface IPostRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        Task<bool> SaveAll();
        Task<IEnumerable<Post>> GetPosts();
        Task<Post> GetPost(int id);
    }
}