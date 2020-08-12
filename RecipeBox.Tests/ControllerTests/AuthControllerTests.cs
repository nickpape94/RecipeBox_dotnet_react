using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using RecipeBox.API.Controllers;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;
using Xunit;

namespace RecipeBox.Tests
{
    public class AuthControllerTests
    {
        private Mock<IAuthRepository> _authRepoMock;
        private Mock<IRecipeRepository> _recipeRepoMock;
        private Mock<IConfiguration> _configMock;
        private AuthController _authController;
        private readonly ClaimsPrincipal _userClaims;
        public AuthControllerTests()
        {
            _authRepoMock = new Mock<IAuthRepository>();
            // (MockBehavior.Strict)
            _recipeRepoMock = new Mock<IRecipeRepository>();
            _configMock = new Mock<IConfiguration>();

            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfiles()); });

            var mapper = mockMapper.CreateMapper();

            _configMock.Setup(x => x.GetSection("AppSettings:Token").Value).Returns("my super secret key");

            _authController = new AuthController(_authRepoMock.Object, _recipeRepoMock.Object ,_configMock.Object, mapper);

            // Mock claim types
            _userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "josh"),
                new Claim(ClaimTypes.NameIdentifier, "2"),
            }, "mock"));
            _authController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _userClaims }
            };
        }

        // [Fact]
        // public void Register_Should_Return_CreatedAtRoute_When_User_Created()
        // {
        //     // Arrange
        //     string username = "George";
        //     string email = "George1@fake-mail.com";
        //     string password = "password123";
        //     // Random rand = new Random();
        //     // byte[] passwordHash = new byte[64]; 
        //     // byte[] passwordSalt = new byte[128];
        //     // rand.NextBytes(passwordHash); 
        //     var userToCreate = new User {
        //         Username = username,
        //         Email = email
        //     };
        //     var userForRegisterDto = new UserForRegisterDto
        //     {
        //         Username = username,
        //         Email = email,
        //         Password = password
        //     };
        //     var userToReturn = new User {
        //         // UserId = 1,
        //         Username = username,
        //         Email = email.ToLower(),
        //         PasswordHash = It.IsAny<byte[]>(),
        //         PasswordSalt = It.IsAny<byte[]>()
        //     };
            
        //     _authRepoMock.Setup(x => x.UserExists(username.ToLower()))
        //         .Returns(() => Task.FromResult(false));
        //     // _authRepoMock.Setup(x => x.Register(userToCreate, password)).ReturnsAsync(userToReturn);
        //     // _authRepoMock.Setup(x => x.Register(userToCreate, password)).Returns(Task.FromResult(userToReturn));


        //     // Act
        //     var result = _authController.Register(userForRegisterDto).Result;

        //     // Assert
        //     var okResult = Assert.IsType<CreatedAtRouteResult>(result);
        //     // Assert.Equal(new StatusCodeResult(201).StatusCode, result.StatusCode);
        // }

        [Fact]
        public void Register_Should_Return_BadRequest_When_Email_Already_Exists()
        {
            // Arrange
            string username = "james";
            string email = "james1@fakemail.com";
            _authRepoMock.Setup(x => x.UserExists(email.ToLower()))
                .Returns(() => Task.FromResult(true));

            // Act
            var result = _authController.Register(new UserForRegisterDto
            {
                Username = username,
                Email = email,
                Password = "password123"
            }).Result;

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Email already in use", okResult.Value);
        }

        [Fact]
        public void Login_Should_Return_OkObjectResult_With_Token_When_Correct_Login_Data()
        {
            // Arrange
            string username = "Bob";
            string email = "bob123@fakemail.com";
            string password = "password123";
            _authRepoMock.Setup(x => x.Login(email.ToLower(), password))
                .Returns(Task.FromResult( new User { UserId = 1, Username = username, Email = email}));
            
            // Act
            var result = _authController.Login( new UserForLoginDto
            {
                Email = email,
                Password = password
            }).Result as OkObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(result.Value);
        }
       
        [Fact]
        public void Login_Should_Return_Unauthorized_If_Credentials_Invalid()
        {
            // Arrange
            string email = "bob123@fakemail.com";
            string password = "An extremely long and highly incorrect password";
            _authRepoMock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => null);
            
            // Act
            var result = _authController.Login( new UserForLoginDto
            {
                Email = email,
                Password = password
            }).Result;

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void ChangePassword_UnauthorizedUserClaims_ReturnsUnauthorized()
        {
            // arrange
            var userId = 1;
            var userFromRepo = FakeUsers().SingleOrDefault(x => x.UserId == userId);

            // act
            _recipeRepoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);

            var result = _authController.ChangePassword(userId, new PasswordForChangeDto {
                OldPassword = "password",
                NewPassword = "password1"
            }).Result;

            // assert
            Assert.IsType<UnauthorizedResult>(result);
        }
        
        [Fact]
        public void ChangePassword_Success_ReturnsOk()
        {
            // arrange
            var userId = 2;
            var oldPassword = "password";
            var newPassword = "new password";
            var userFromRepo = FakeUsers().SingleOrDefault(x => x.UserId == userId);
            

            // act
            _recipeRepoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _authRepoMock.Setup(x => x.ResetPassword(userId, oldPassword, newPassword)).ReturnsAsync(userFromRepo);

            var result = _authController.ChangePassword(userId, new PasswordForChangeDto {
                OldPassword = oldPassword,
                NewPassword = newPassword
            }).Result;

            // assert
            Assert.IsType<OkObjectResult>(result);
        }

        private ICollection<User> FakeUsers()
        {
            return new List<User>()
            {
                new User
                {
                    UserId = 1,
                    Username = "nick",
                },
                new User
                {
                    UserId = 2,
                    Username = "josh"
                }
            };
        }

    }

    
}