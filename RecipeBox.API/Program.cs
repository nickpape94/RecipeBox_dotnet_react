using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RecipeBox.API.Data;
using RecipeBox.API.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace RecipeBox.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            using ( var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DataContext>();
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    context.Database.Migrate();
                    Seed.SeedUsers(userManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occured during migration");
                }
            }

            // Execute().Wait();
            host.Run();
            
        }

        // static async Task Execute()
        // {
        //     var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        //     // var apiKey = Configuration.GetSection("AppSettings:Token").Value;
        //     Console.WriteLine(apiKey);
        //     var client = new SendGridClient(apiKey);
        //     var from = new EmailAddress("test@example.com", "Nick-");
        //     var subject = "Sending with SendGrid is Fun";
        //     var to = new EmailAddress("test@example.com", "Nick-");
        //     var plainTextContent = "and easy to do anywhere, even with C#";
        //     var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        //     var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        //     var response = await client.SendEmailAsync(msg);
        // }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
