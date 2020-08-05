using System.Threading.Tasks;
using RecipeBox.API.Models;

namespace RecipeBox.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string email, string password);
        Task<User> ResetPassword(int userId, string oldPassword, string newPassword);
        Task<bool> UserExists(string email);
    }
}