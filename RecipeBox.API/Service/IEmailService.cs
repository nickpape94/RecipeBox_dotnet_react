using System.Threading.Tasks;

namespace RecipeBox.API.Service
{
    public interface IEmailService
    {
        Task ResetPasswordAsync(string toEmail, string subject, string content);
    }
}