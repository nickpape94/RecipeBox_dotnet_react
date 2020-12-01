using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using MimeKit;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos.AuthDtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;

namespace RecipeBox.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;
        public EmailService(IRecipeRepository recipeRepo, UserManager<User> userManager, IConfiguration config)
        {
            _config = config;
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

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _recipeRepo.GetUser(email);

            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with this email"
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);

            var validToken = WebEncoders.Base64UrlEncode(encodedToken);
            // string url = $"localhost:5000/api/auth/resetpassword?email={email}&token={validToken}";

            string url = $"localhost:3000/reset-password?email={email}&token={validToken}";

            await SendEmailAsync(email, "Password reset", $"<p>Hi {user.UserName}, You recently requested to reset your password</p> <br> <p>Please follow the link below to reset your password</p> <br> <a href='{url}'>Reset Password</a> <br> <p>If this wasn't you, please ignore this email</p> <br> <p>Regards, RecipeBox</p>");

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Reset password URL has been sent to the email successfully"
            };


        }

        public async Task<UserManagerResponse> ResetPasswordAsync(PasswordForResetDto passwordForResetDto)
        {
            var user = await _recipeRepo.GetUser(passwordForResetDto.Email);
            
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with this email"
                };

            

            var decodedToken = WebEncoders.Base64UrlDecode(passwordForResetDto.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, normalToken, passwordForResetDto.Password);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Password has been reset successfully"
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Something went wrong",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        // public async Task SendEmailAsync(string toEmail, string subject, string content)
        // {
        //     var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        //     // Console.WriteLine(apiKey);
        //     var client = new SendGridClient(apiKey);
        //     var from = new EmailAddress("recipeboxbot@gmail.com", "RecipeBox");
        //     var to = new EmailAddress(toEmail);
        //     var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
        //     var response = await client.SendEmailAsync(msg);
        // }
        
        public Task SendEmailAsync(string toEmail, string subject, string content)
        {
            // Console.WriteLine( _config.GetSection("MailKitSettings:Password").Value);
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Recipe Box", "recipeboxbot@gmail.com"));

            message.To.Add(MailboxAddress.Parse(toEmail));

            message.Subject = subject;

            message.Body = new TextPart("html"){
                Text = content
            };

            using(var client = new SmtpClient()){

                client.Connect("smtp.gmail.com", 587, false);

                client.Authenticate("recipeboxbot@gmail.com",  _config.GetSection("MailKitSettings:Password").Value);

                client.Send(message);

                client.Disconnect(true);
            }

            return Task.CompletedTask;
        }
    }
}