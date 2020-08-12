using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IRecipeRepository _recipeRepo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        
        public AuthController(IAuthRepository authRepo, IRecipeRepository recipeRepo, IConfiguration config, IMapper mapper)
        {
            _recipeRepo = recipeRepo;
            _authRepo = authRepo; 
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Email = userForRegisterDto.Email.ToLower();
            
            if ( await _authRepo.UserExists(userForRegisterDto.Email))
                return BadRequest("Email already in use");

            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            // var userToCreate = new User{
            //     Username = userForRegisterDto.Username,
            //     Created = userForRegisterDto.Created,
            //     LastActive = userForRegisterDto.LastActive
            // }; 

            // Testing fails on createdUser
            var createdUser = await _authRepo.Register(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

            return CreatedAtRoute("GetUser", new {controller = "Users", id = createdUser.UserId}, userToReturn);

            // return StatusCode(201);
        }

        [HttpPost("login", Name = "Login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _authRepo.Login(userForLoginDto.Email.ToLower(), userForLoginDto.Password);


            if (userFromRepo == null) return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.UserId.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
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
            await _authRepo.SendEmailAsync(userForLoginDto.Email, "Password reset", $"<p>Hi {userFromRepo.Username}, You recently requested to reset your password</p> <br> <p>Please follow the link below to reset your password</p> <br> <a href=https://www.w3schools.com/ >Link to my website</a> <br> <p>If this wasn't you, please ignore this email</p> <br> <p>Regards, RecipeBox</p>");

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }

        [HttpPost("user/{userId}/changePassword")]
        public async Task<IActionResult> ChangePassword(int userId, PasswordForChangeDto passwordForChangeDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var user = await _recipeRepo.GetUser(userId);

            // var passwordToChange = _mapper.Map<User>(passwordForChangeDto);

            var updatedUserPassword = await _authRepo.ResetPassword(userId, passwordForChangeDto.OldPassword ,passwordForChangeDto.NewPassword);

            var userToReturn = _mapper.Map<UserForDetailedDto>(updatedUserPassword);

            // Relogin user back in if logged out during password reset
            var userForLogin = new UserForLoginDto
            {
                Email = user.Email,
                Password = passwordForChangeDto.NewPassword
            };

            var loginWithNewPassword = Login(userForLogin);

            return Ok(userToReturn);

        }
    }
}