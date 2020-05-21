using System.Threading.Tasks;
using RecipeBox.API.src.Main.Models;

namespace RecipeBox.API.src.Main.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}