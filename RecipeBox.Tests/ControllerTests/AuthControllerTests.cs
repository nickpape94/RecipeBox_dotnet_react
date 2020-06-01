using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using RecipeBox.API.Controllers;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Models;
using Xunit;

namespace RecipeBox.Tests
{
    public class AuthControllerTests
    {
        private Mock<IAuthRepository> _repoMock;
        private Mock<IConfiguration> _configMock;
        private AuthController _authController;

        public AuthControllerTests()
        {
            _repoMock = new Mock<IAuthRepository>();
            _configMock = new Mock<IConfiguration>();

            _configMock.Setup(x => x.GetSection("AppSettings:Token").Value).Returns("my super secret key");

            _authController = new AuthController(_repoMock.Object, _configMock.Object);
        }

        [Fact]
        public void Register_Should_Return_201StatusCode_When_User_Created()
        {
            // Arrange
            string username = "George";
            string password = "password123";
            _repoMock.Setup(x => x.UserExists(username.ToLower()))
                .Returns(() => Task.FromResult(false));

            // Act
            var result = _authController.Register(new UserForRegisterDto
            {
                Username = username,
                Password = password
            }).Result as StatusCodeResult;

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(new StatusCodeResult(201).StatusCode, result.StatusCode);
        }

        [Fact]
        public void Register_Should_Return_BadRequest_When_Username_Already_Exists()
        {
            // Arrange
            string username = "james";

            _repoMock.Setup(x => x.UserExists(username.ToLower()))
                .Returns(() => Task.FromResult(true));

            // Act
            var result = _authController.Register(new UserForRegisterDto
            {
                Username = "james",
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
            string password = "password123";
            _repoMock.Setup(x => x.Login(username.ToLower(), password))
                .Returns(Task.FromResult( new User { UserId = 1, Username = username}));
            
            // Act
            var result = _authController.Login( new UserForLoginDto
            {
                Username = username,
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
            string username = "Bob";
            string password = "An extremely long and highly incorrect password";
            _repoMock.Setup(x => x.Login(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(() => null);
            
            // Act
            var result = _authController.Login( new UserForLoginDto
            {
                Username = username,
                Password = password
            }).Result;

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}