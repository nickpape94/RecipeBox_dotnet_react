using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RecipeBox.API.Service
{
    public class EmailService : IEmailService
    {
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