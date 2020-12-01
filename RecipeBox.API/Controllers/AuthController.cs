using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos.AuthDtos;
using RecipeBox.API.Dtos.UserDtos;
using RecipeBox.API.Models;
using RecipeBox.API.Services;

namespace RecipeBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailService _emailService;
        
        public AuthController(IRecipeRepository recipeRepo, IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IEmailService emailService)
        {
            _emailService = emailService;
            _signInManager = signInManager;
            _userManager = userManager;
            _recipeRepo = recipeRepo;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);

            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(userToCreate);

                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);

                string url = $"localhost:5000/api/auth/confirmemail?userid={userToCreate.Id}&token={validEmailToken}";

                await _emailService.SendEmailAsync(userToCreate.Email, "Confirm your email", 
                $"<h3>Welcome to Recipe Box</h3> <p>Please confirm your email by <a href='{url}'>Clicking here</a></p>");

                // return CreatedAtRoute("GetUser", new {controller = "Users", id = userToCreate.Id}, userToReturn);
                // return CreatedAtRoute("Login", new {controller = "Users", id = userToCreate.Id}, userToReturn);
                var user = await _recipeRepo.GetUser(userToReturn.Id);

                return Ok(new {
                    token = GenerateJwtToken(user)
                    // user = appUser
                });
            }

            return BadRequest(result.Errors);

        }

        // /api/auth/confirmemail?userid&token
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            var user = await _recipeRepo.GetUser(userId);
            
            if (user == null || string.IsNullOrWhiteSpace(token)) return NotFound();

            var result = await _emailService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                // Need to come back to this to return an HTML page!
                // return Ok("Thanks for confirming your email");
                return Redirect($"http://localhost:3000/email-confirmed?userid={userId}&token={token}");
            }

            return BadRequest(result);
        }

        [HttpPost("login", Name = "Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(userForLoginDto.Email);

            if (user == null) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (result.Succeeded)
            {
                user.LastActive = DateTime.Now;
            
                await _recipeRepo.SaveAll();
                var appUser = _mapper.Map<UserForDetailedDto>(user);

                return Ok(new {
                    token = GenerateJwtToken(user),
                    user = appUser
                });
            }

            return Unauthorized();
        }

        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody]string email)
        {
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email)) return BadRequest("Invalid email");

            var userFromRepo = await _recipeRepo.GetUser(email);
            if (userFromRepo == null) return BadRequest("User not found");

            var result = await _emailService.ForgetPasswordAsync(userFromRepo.Email);
            
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
        
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(PasswordForResetDto passwordForResetDto)
        {
            var result = await _emailService.ResetPasswordAsync(passwordForResetDto);
            var user = await _recipeRepo.GetUser(passwordForResetDto.Email);

            if (result.IsSuccess)
                return Ok(new {
                    token = GenerateJwtToken(user)
                });

            return BadRequest("Failed to update password");
        }
       
        [HttpPost("user/{userId}/changePassword")]
        public async Task<IActionResult> ChangePassword(int userId, PasswordForChangeDto passwordForChangeDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var user = await _recipeRepo.GetUser(userId);

            // if (passwordForChangeDto.NewPassword != passwordForChangeDto.OldPassword) return BadRequest("Passwords do not match");

            var result = await _userManager.ChangePasswordAsync(user, passwordForChangeDto.OldPassword, passwordForChangeDto.NewPassword);

            if (result.Succeeded)
            {
                var userToReturn = _mapper.Map<UserForDetailedDto>(user);

                return Ok(new {
                    token = GenerateJwtToken(user),
                    user = userToReturn
                });
            }

            return Unauthorized();


        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}