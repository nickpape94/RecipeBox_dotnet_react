using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Models;

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
        
        public AuthController(IRecipeRepository recipeRepo, IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
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
                return CreatedAtRoute("GetUser", new {controller = "Users", id = userToCreate.Id}, userToReturn);
            }

            return BadRequest(result.Errors);

        }

        [HttpPost("login", Name = "Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(userForLoginDto.Email);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

            if (result.Succeeded)
            {
                var appUser = _mapper.Map<UserForListDto>(user);

                return Ok(new {
                    token = GenerateJwtToken(user),
                    user = appUser
                });
            }

            return Unauthorized();
        }
       
        // [HttpPost("user/{userId}/changePassword")]
        // public async Task<IActionResult> ChangePassword(int userId, PasswordForChangeDto passwordForChangeDto)
        // {
        //     if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

        //     var user = await _recipeRepo.GetUser(userId);

        //     // var passwordToChange = _mapper.Map<User>(passwordForChangeDto);

        //     var updatedUserPassword = await _authRepo.ResetPassword(userId, passwordForChangeDto.OldPassword ,passwordForChangeDto.NewPassword);

        //     var userToReturn = _mapper.Map<UserForDetailedDto>(updatedUserPassword);

        //     // Relogin user back in if logged out during password reset
        //     var userForLogin = new UserForLoginDto
        //     {
        //         Email = user.Email,
        //         Password = passwordForChangeDto.NewPassword
        //     };

        //     var loginWithNewPassword = Login(userForLogin);

        //     return Ok(userToReturn);

        // }

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