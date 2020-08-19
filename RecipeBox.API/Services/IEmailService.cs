using System.Threading.Tasks;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;

namespace RecipeBox.API.Services
{
    public interface IEmailService
    {
        Task<UserManagerResponse> ConfirmEmailAsync(int userId, string token);
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}