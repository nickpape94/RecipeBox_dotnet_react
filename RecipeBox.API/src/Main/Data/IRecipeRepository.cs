using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeBox.API.src.Main.Models;

namespace RecipeBox.API.src.Main.Data
{
    public interface IRecipeRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);
    }
}