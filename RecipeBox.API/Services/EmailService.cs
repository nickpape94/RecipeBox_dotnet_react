using System;
using System.Threading.Tasks;
using RecipeBox.API.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RecipeBox.API.Services
{
    public class EmailService : IEmailService
    {
        public async Task<User> ConfirmEmailAsync(int userId, string token)
        {
            return null;
        }

        public async Task ResetPasswordAsync(string toEmail, string subject, string content)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            // Console.WriteLine(apiKey);
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("recipeboxbot@gmail.com", "RecipeBox");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }
    }
}