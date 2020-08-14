using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.API.Data;
using RecipeBox.API.Service;

namespace RecipeBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IRecipeRepository _recipeRepo;

        public EmailController(IEmailService emailService, IRecipeRepository recipeRepo)
        {
            _recipeRepo = recipeRepo;
            _emailService = emailService;
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]string email)
        {
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email)) return BadRequest("Invalid email");

            var userFromRepo = await _recipeRepo.GetUser(email);
            if (userFromRepo == null) return BadRequest("User not found");

            // var token = await _userManger.GeneratePasswordResetTokenAsync(userFromRepo);

            // send an email to user with password reset link
            await _emailService.ResetPasswordAsync(userFromRepo.Email, "Password reset", $"<p>Hi {userFromRepo.Username}, You recently requested to reset your password</p> <br> <p>Please follow the link below to reset your password</p> <br> <a href=https://www.w3schools.com/>Reset Password</a> <br> <p>If this wasn't you, please ignore this email</p> <br> <p>Regards, RecipeBox</p>");
            
            // user updates their password, password gets hashed and stores to the database
            // link should expire after password has been updated
            // log user back in with their newly updated password

            return Ok("test");
        }
    }
}