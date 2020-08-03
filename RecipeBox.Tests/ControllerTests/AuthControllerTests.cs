using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private Mock<IAuthRepository> _repoMock;
        private Mock<RecipeRepository> _recipeMock;
        private Mock<IConfiguration> _configMock;
        private AuthController _authController;

        public AuthControllerTests()
        {
            _repoMock = new Mock<IAuthRepository>(MockBehavior.Strict);
            _configMock = new Mock<IConfiguration>();

            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfiles()); });

            var mapper = mockMapper.CreateMapper();

            _configMock.Setup(x => x.GetSection("AppSettings:Token").Value).Returns("my super secret key");

            _authController = new AuthController(_repoMock.Object, _configMock.Object, mapper);
        }

        // [Fact]
        // public void Register_Should_Return_CreatedAtRoute_When_User_Created()
        // {
        //     // Arrange
        //     string username = "George";
        //     string password = "password123";
        //     Random rand = new Random();
        //     byte[] passwordHash = new byte[64]; 
        //     byte[] passwordSalt = new byte[128];
        //     rand.NextBytes(passwordHash); 
        //     var userToSubmit = new User {
        //         Username = username,
        //     };
        //     var userToReturn = new User {
        //         UserId = 1,
        //         Username = username,
        //         PasswordHash = passwordHash,
        //         PasswordSalt = passwordSalt
        //     };
            
        //     _repoMock.Setup(x => x.UserExists(username.ToLower()))
        //         .Returns(() => Task.FromResult(false));
        //     _repoMock.Setup(x => x.Register(userToSubmit, password)).Returns(Task.FromResult(userToReturn));


        //     // Act
        //     var result = _authController.Register(new UserForRegisterDto
        //     {
        //         Username = username,
        //         Password = password
        //     }).Result;

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
            _repoMock.Setup(x => x.UserExists(email.ToLower()))
                .Returns(() => Task.FromResult(true));

            // Act
            var result = _authController.Register(new UserForRegisterDto
            {
                Username = username,
                Email = email,
                Password = "password123"
            }).Result;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Login_Should_Return_OkObjectResult_With_Token_When_Correct_Login_Data()
        {
            // Arrange
            string username = "Bob";
            string email = "bob123@fakemail.com";
            string password = "password123";
            _repoMock.Setup(x => x.Login(email.ToLower(), password))
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
            _repoMock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>()))
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

    }

    
}