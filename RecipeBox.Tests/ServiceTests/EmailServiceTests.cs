using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Moq;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos.AuthDtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;
using RecipeBox.API.Services;
using RecipeBox.Tests.Helpers;
using Xunit;

namespace RecipeBox.Tests.ServiceTests
{
    public class EmailServiceTests
    {
        private Mock<IRecipeRepository> _repoMock;
        private Mock<IConfiguration> _configMock;
        private Mock<FakeUserManager> _mockUserManager;
        // private Mock<IdentityResult> _mockIdentityResult;
        private Mock<IEmailService> _mockEmailService;
        private EmailService _emailService;
        public EmailServiceTests()
        {
            _repoMock = new Mock<IRecipeRepository>();
            _configMock = new Mock<IConfiguration>();
            _mockUserManager = new Mock<FakeUserManager>();
            _mockEmailService = new Mock<IEmailService>();

            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            var emailConfig = config.GetSection("MailKitSettings:Password");

            _configMock.Setup(x => x.GetSection("MailKitSettings:Password").Value).Returns(emailConfig.Value);
            
            _emailService = new EmailService(_repoMock.Object, _mockUserManager.Object, _configMock.Object);

        }

        [Fact]
        public void ConfirmEmailAsync_UserNotFound()
        {
            var result = _emailService.ConfirmEmailAsync(1, "randomtoken").Result;

            var okResult = Assert.IsType<UserManagerResponse>(result);
            Assert.False(okResult.IsSuccess);
            Assert.Equal("User not found", okResult.Message);
        }

        [Fact]
        public void ConfirmEmailAsync_Success()
        {
            // Arrange
            var userId = 1;
            var token = "gheirnfnsgheirnfnsgheirnfnsgheirnfns";
            var user = new User()
            {
                Id = userId,
                UserName = "Nick"
            };

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(user);
            
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            // Act
            var result = _emailService.ConfirmEmailAsync(userId, token).Result;

            // Assert
            var okResult = Assert.IsType<UserManagerResponse>(result);
            Assert.True(okResult.IsSuccess);
            Assert.Equal("Email confirmed successfully!", okResult.Message);
        }
        
        [Fact]
        public void ConfirmEmailAsync_Fails()
        {
            // Arrange
            var userId = 1;
            var token = "gheirnfnsgheirnfnsgheirnfnsgheirnfns";
            var user = new User()
            {
                Id = userId,
                UserName = "Nick"
            };

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(user);

            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, It.IsAny<string>())).Returns(Task.FromResult(IdentityResult.Failed())).Verifiable();


            // Act
            var result = _emailService.ConfirmEmailAsync(userId, token).Result;

            // Assert
            var okResult = Assert.IsType<UserManagerResponse>(result);
            Assert.False(okResult.IsSuccess);
            Assert.Equal("Email was not confirmed", okResult.Message);
        }

        [Fact]
        public void ForgetPasswordAsync_UserNull()
        {
            // Arrange

            // Act
            var result = _emailService.ForgetPasswordAsync("fakeuser@fakemail.com").Result;

            // Assert
            var okResult = Assert.IsType<UserManagerResponse>(result);
            Assert.False(okResult.IsSuccess);
            Assert.Equal("No user associated with this email", okResult.Message);
        }
        
        [Fact]
        public void ForgetPasswordAsync_Success()
        {
            // Arrange
            var email = "fakeuser123@1234543fakemailss.com";
            var user = new User() {
                Email = email,
                UserName = "Nick",
                Id = 1
            };

            _repoMock.Setup(x => x.GetUser(email)).ReturnsAsync(user);
            
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("some random value");
            
            // _mockEmailService.Setup(x => x.SendEmailAsync(email, "subject", "content")).Returns(Task.CompletedTask);

            // Act
            var result = _emailService.ForgetPasswordAsync(email).Result;

            // Assert
            var okResult = Assert.IsType<UserManagerResponse>(result);
            Assert.True(okResult.IsSuccess);
            Assert.Equal("Reset password URL has been sent to the email successfully", okResult.Message);
        }

        [Fact]
        public void ResetPassword_User_Null()
        {
            // Arrange
            var passwordForResetDto = new PasswordForResetDto(){
                Token = "randomtoken",
                Email = "fakeemail109@123fakeemail123.com",
                Password = "password123"
            };

            // Act
            var result = _emailService.ResetPasswordAsync(passwordForResetDto).Result;

            // Assert
            var okResult = Assert.IsType<UserManagerResponse>(result);
            Assert.False(okResult.IsSuccess);
            Assert.Equal("No user associated with this email", okResult.Message);
        }
        
        [Fact]
        public void ResetPassword_Success()
        {
            // Arrange
            var passwordForResetDto = new PasswordForResetDto(){
                Token = "randomtoken",
                Email = "fakeemail109@123fakeemail123.com",
                Password = "password123"
            };
            var user = new User() {
                UserName = "Nick",
                Id = 1
            };

            _repoMock.Setup(x => x.GetUser(passwordForResetDto.Email)).ReturnsAsync(user);

            _mockUserManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), passwordForResetDto.Password)).Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            // Act
            var result = _emailService.ResetPasswordAsync(passwordForResetDto).Result;

            // Assert
            var okResult = Assert.IsType<UserManagerResponse>(result);
            Assert.True(okResult.IsSuccess);
            Assert.Equal("Password has been reset successfully", okResult.Message);
        }
        
        [Fact]
        public void ResetPassword_Fails()
        {
            // Arrange
            var passwordForResetDto = new PasswordForResetDto(){
                Token = "randomtoken",
                Email = "fakeemail109@123fakeemail123.com",
                Password = "password123"
            };
            var user = new User() {
                UserName = "Nick",
                Id = 1
            };

            _repoMock.Setup(x => x.GetUser(passwordForResetDto.Email)).ReturnsAsync(user);

            _mockUserManager.Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), passwordForResetDto.Password)).Returns(Task.FromResult(IdentityResult.Failed())).Verifiable();

            // Act
            var result = _emailService.ResetPasswordAsync(passwordForResetDto).Result;

            // Assert
            var okResult = Assert.IsType<UserManagerResponse>(result);
            Assert.False(okResult.IsSuccess);
            Assert.Equal("Something went wrong", okResult.Message);
        }
    }
}