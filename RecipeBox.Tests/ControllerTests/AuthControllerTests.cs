using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using RecipeBox.API.Controllers;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Dtos.AuthDtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;
using RecipeBox.API.Services;
using RecipeBox.Tests.Helpers;
using Xunit;

namespace RecipeBox.Tests
{
    public class AuthControllerTests
    {
        private Mock<IRecipeRepository> _recipeRepoMock;
        private Mock<IConfiguration> _configMock;
        private Mock<FakeUserManager> _mockUserManager;
        private Mock<FakeSignInManager> _mockSignInManager;
        private Mock<IEmailService> _mockEmailService;
        private AuthController _authController;
        private readonly ClaimsPrincipal _userClaims;
        public AuthControllerTests()
        {
            // (MockBehavior.Strict)
            _recipeRepoMock = new Mock<IRecipeRepository>();
            _configMock = new Mock<IConfiguration>();
            _mockEmailService = new Mock<IEmailService>();

            var userStoreMock = new Mock<IUserStore<User>>();

            _mockUserManager = new Mock<FakeUserManager>();
            _mockSignInManager = new Mock<FakeSignInManager>();

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();


            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfiles()); });

            var mapper = mockMapper.CreateMapper();

            _configMock.Setup(x => x.GetSection("AppSettings:Token").Value).Returns("my super secret key");

            _authController = new AuthController(_recipeRepoMock.Object ,_configMock.Object, mapper, _mockUserManager.Object, _mockSignInManager.Object, _mockEmailService.Object);

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

        [Fact]
        public void Register_Should_Return_CreatedAtRoute_When_User_Created()
        {
            // Arrange
            var userToCreate = new User
            {
                Email = "nick@fakemail.com",
                UserName = "nick"
            };

            // var fakeToken = "Q2ZESjhFVE5VYVBEb3FOQWhNbDduWHRuUXF3OWZZdDMva2dtUU9zTlJZR3JXcytXOG54MXF6bVc2UktDeUdZbVk1RUlHYTduUmRmM0o5Vkp4M2dTMHJZeHNzN0FPMGFFNE9YbGxCL0JiTzZ1TWZEb0RBME4yMERGVFdraUNXYWtOUkR1OFJpTXF3dkEzUEltRFh0Wm8wbDNLSnZqU21INlBNYytGdDEzLzFIUE5QNDc2V2NYZWVyT2pHVjNKOWI3K25Kc0ZnPT0";

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), "password123")).Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
           
            _mockUserManager.Setup(x => x.FindByEmailAsync(userToCreate.Email)).Returns(Task.FromResult(userToCreate));
            
            _mockUserManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>())).ReturnsAsync("ervipevpievpjevpoj").Verifiable();

            // Act
            var result = _authController.Register(new UserForRegisterDto
            {
                Email = "nick@fakemail.com",
                Username = "nick",
                Password = "password123"
            }
            ).Result;

            // Assert
            var okResult = Assert.IsType<CreatedAtRouteResult>(result);
        }

        [Fact]
        public void Register_Should_Return_BadRequest_When_Errors_Occur()
        {
             // Arrange
            var userToCreate = new User
            {
                Email = "nick@fakemail.com",
                UserName = "nick"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), "password123")).Returns(Task.FromResult(IdentityResult.Failed())).Verifiable();

            // Act
            var result = _authController.Register(new UserForRegisterDto
            {
                Email = "nick@fakemail.com",
                Username = "nick",
                Password = "password123"
            }
            ).Result;

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(new List<IdentityError>(), okResult.Value);
        }

        [Fact]
        public void Confirm_Email_User_Not_Found_Returns_Not_Found()
        {
            // Arrange
            int userId = 10;
            _recipeRepoMock.Setup(x => x.GetUser(userId));

            // Act
            var result = _authController.ConfirmEmail(userId, It.IsAny<string>()).Result;

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        
        [Fact]
        public void Confirm_Email_Successful_ReturnsOk()
        {
            // Arrange
            int userId = 10;
            _recipeRepoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(new User
            {
                Email = "nick@fakemail.com",
                UserName = "nick"
            });
            _mockEmailService.Setup(x => x.ConfirmEmailAsync(userId, It.IsAny<string>())).ReturnsAsync(new UserManagerResponse
            {
                Message = "success",
                IsSuccess = true
            });

            // Act
            var result = _authController.ConfirmEmail(userId, "somerandomstring").Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Thanks for confirming your email!", okResult.Value);
        }
        
        [Fact]
        public void Confirm_Email_Fails_Returns_BadRequest()
        {
            // Arrange
            int userId = 10;
            var response = new UserManagerResponse 
            { Errors = null, ExpireDate = null, IsSuccess = false, Message = "email confirmation failed" 
            };
            _recipeRepoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(new User
            {
                Email = "nick@fakemail.com",
                UserName = "nick"
            });
            _mockEmailService.Setup(x => x.ConfirmEmailAsync(userId, It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = _authController.ConfirmEmail(userId, "somerandomstring").Result;

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public void Login_Should_Return_OkObjectResult_With_Token_When_Correct_Login_Data()
        {
            // Arrange
            var userToLogin = new UserForLoginDto
            {
                Email = "timmy2@fakemail.com",
                Password = "password123"
            };
            var user = new User
            {
                Email = userToLogin.Email,
                UserName = "tim"
            };
            _mockUserManager.Setup(x => x.FindByEmailAsync(userToLogin.Email)).Returns(Task.FromResult(user));
            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, userToLogin.Password, false)).Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));
            
            // Act
            var result = _authController.Login(userToLogin).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Assert.Equal(okResult.Value, new {
            //     token = Type<Jw>,
            //     user = "RecipeBox.API.Dtos.UserForListDto"
            // });
        }
       
        [Fact]
        public void Login_Should_Return_Unauthorized_If_Credentials_Invalid()
        {
            // Arrange
            var userToLogin = new UserForLoginDto
            {
                Email = "timmy2@fakemail.com",
                Password = "password123"
            };
            var user = new User
            {
                Email = userToLogin.Email,
                UserName = "tim"
            };
            _mockUserManager.Setup(x => x.FindByEmailAsync(userToLogin.Email)).Returns(Task.FromResult(user));
            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, userToLogin.Password, false)).Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Failed));
            
            // Act
            var result = _authController.Login(userToLogin).Result;

            // Assert
            var okResult = Assert.IsType<UnauthorizedResult>(result);
            // Assert.Equal(okResult.Value, new {
            //     token = Type<Jw>,
            //     user = "RecipeBox.API.Dtos.UserForListDto"
            // });
        }

        [Fact]
        public void ForgetPassword_InvalidEmail_ReturnsBadRequest()
        {
            var email = "invalidemail.com";

            var result = _authController.ForgetPassword(email).Result;

            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid email", okResult.Value);

        }
        
        [Fact]
        public void ForgetPassword_NoMatchingUser_ReturnsBadRequest()
        {
            var email = "validemail@fakemail.com";

            var result = _authController.ForgetPassword(email).Result;

            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User not found", okResult.Value);

        }
        
        [Fact]
        public void ForgetPassword_FindsUser_SendsEmail_ReturnsOk()
        {
            var email = "validemail@fakemail.com";
            var response = new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Reset password URL has been sent to the email successfully"
            };
            _recipeRepoMock.Setup(x => x.GetUser(email)).ReturnsAsync(
                new User
                {
                    UserName = "user1",
                    Email = email
                }
            );
            _mockEmailService.Setup(x => x.ForgetPasswordAsync(email)).ReturnsAsync(response);

            var result = _authController.ForgetPassword(email).Result;

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);

        }
        
        [Fact]
        public void ForgetPassword_FindsUser_FailsToSendsEmail_ReturnsBadRequest()
        {
            var email = "validemail@fakemail.com";
            var response = new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Failed to send email to user"
            };
            _recipeRepoMock.Setup(x => x.GetUser(email)).ReturnsAsync(
                new User
                {
                    UserName = "user1",
                    Email = email
                }
            );
            _mockEmailService.Setup(x => x.ForgetPasswordAsync(email)).ReturnsAsync(response);

            var result = _authController.ForgetPassword(email).Result;

            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(response, okResult.Value);

        }

        [Fact]
        public void ResetPassword_Success_ReturnsOk()
        {
            var passwordForResetDto = new PasswordForResetDto
            {
                Token = It.IsAny<string>(),
                Email = "someemail@fakemail.com",
                NewPassword = "password1",
                ConfirmPassword = "password1"
            };
            var response = new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Password reset was successful"
            };
            
            _mockEmailService.Setup(x => x.ResetPasswordAsync(passwordForResetDto)).ReturnsAsync(response);

            var result = _authController.ResetPassword(passwordForResetDto).Result;

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }
        
        [Fact]
        public void ResetPassWord_Fails_ReturnsBadRequest()
        {
            var passwordForResetDto = new PasswordForResetDto
            {
                Token = It.IsAny<string>(),
                Email = "someemail@fakemail.com",
                NewPassword = "password1",
                ConfirmPassword = "password1"
            };
            var response = new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Password reset failed"
            };
            
            _mockEmailService.Setup(x => x.ResetPasswordAsync(passwordForResetDto)).ReturnsAsync(response);

            var result = _authController.ResetPassword(passwordForResetDto).Result;

            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        // [Fact]
        // public void ChangePassword_UnauthorizedUserClaims_ReturnsUnauthorized()
        // {
        //     // arrange
        //     var userId = 1;
        //     var userFromRepo = FakeUsers().SingleOrDefault(x => x.Id == userId);

        //     // act
        //     _recipeRepoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);

        //     var result = _authController.ChangePassword(userId, new PasswordForChangeDto {
        //         OldPassword = "password",
        //         NewPassword = "password1"
        //     }).Result;

        //     // assert
        //     Assert.IsType<UnauthorizedResult>(result);
        // }
        
        // [Fact]
        // public void ChangePassword_Success_ReturnsOk()
        // {
        //     // arrange
        //     var userId = 2;
        //     var oldPassword = "password";
        //     var newPassword = "new password";
        //     var userFromRepo = FakeUsers().SingleOrDefault(x => x.Id == userId);
            

        //     // act
        //     _recipeRepoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
        //     _authRepoMock.Setup(x => x.ResetPassword(userId, oldPassword, newPassword)).ReturnsAsync(userFromRepo);

        //     var result = _authController.ChangePassword(userId, new PasswordForChangeDto {
        //         OldPassword = oldPassword,
        //         NewPassword = newPassword
        //     }).Result;

        //     // assert
        //     Assert.IsType<OkObjectResult>(result);
        // }

        private ICollection<User> FakeUsers()
        {
            return new List<User>()
            {
                new User
                {
                    Id = 1,
                    UserName = "nick",
                    Email = "nick@fakemail.com"
                },
                new User
                {
                    Id = 2,
                    UserName = "josh",
                    Email = "josh@fakemail.com"
                }
            };
        }

    }

    
}