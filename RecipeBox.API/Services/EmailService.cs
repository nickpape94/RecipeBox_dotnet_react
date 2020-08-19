using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RecipeBox.API.Data;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RecipeBox.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly UserManager<User> _userManager;
        public EmailService(IRecipeRepository recipeRepo, UserManager<User> userManager)
        {
            _userManager = userManager;
            _recipeRepo = recipeRepo;
            
        }
        public async Task<UserManagerResponse> ConfirmEmailAsync(int userId, string token)
        {
           var user = await _recipeRepo.GetUser(userId);

           if (user == null) 
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

           var decodedToken = WebEncoders.Base64UrlDecode(token);
           string normalToken = Encoding.UTF8.GetString(decodedToken);

            // fails here
           var result = await _userManager.ConfirmEmailAsync(user, normalToken);

           if (result.Succeeded)
                return new UserManagerResponse
                {
                    Message = "Email confirmed successfully!",
                    IsSuccess = true
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Email was not confirmed",
                Errors = result.Errors.Select(e => e.Description)
            };
        
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
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