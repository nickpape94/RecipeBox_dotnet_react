using System.Threading.Tasks;
using RecipeBox.API.Dtos.AuthDtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;

namespace RecipeBox.API.Services
{
    public interface IEmailService
    {
        Task<UserManagerResponse> ConfirmEmailAsync(int userId, string token);
        Task<UserManagerResponse> ForgetPasswordAsync(string email);
        Task<UserManagerResponse> ResetPasswordAsync(PasswordForResetDto passwordForResetDto);
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}