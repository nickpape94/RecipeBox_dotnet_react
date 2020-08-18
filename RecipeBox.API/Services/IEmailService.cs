using System.Threading.Tasks;
using RecipeBox.API.Models;

namespace RecipeBox.API.Services
{
    public interface IEmailService
    {
        Task<User> ConfirmEmailAsync(int userId, string token);
        Task ResetPasswordAsync(string toEmail, string subject, string content);
    }
}