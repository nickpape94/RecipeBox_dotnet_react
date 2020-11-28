using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RecipeBox.API.Controllers;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Dtos.UserDtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;
using Xunit;

namespace RecipeBox.Tests.ControllerTests
{
    public class UsersControllerTests
    {
        private Mock<IRecipeRepository> _recipeRepoMock;
        private UsersController _usersController;
        private readonly ClaimsPrincipal _userClaims;

        public UsersControllerTests()
        {
            _recipeRepoMock = new Mock<IRecipeRepository>();

            var mockMapper = new MapperConfiguration(cfg => {cfg.AddProfile(
                new AutoMapperProfiles()); });

            var mapper = mockMapper.CreateMapper();

            _usersController = new UsersController(_recipeRepoMock.Object,  mapper);

            // Mock user claims
            _userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "josh"),
                new Claim(ClaimTypes.NameIdentifier, "2"),
            }, "mock"));

            _usersController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _userClaims}
            };
        }

        [Fact]
        public void GetUser_WhenCalled_ReturnsRightUser()
        {
            // Arrange
            int userId = 2;
            var user = GetFakeUserList().SingleOrDefault(x => x.Id == userId);
            
            _recipeRepoMock.Setup(x => x.GetUser(userId))
                .ReturnsAsync(user);

            // Act
            var result = _usersController.GetUser(userId).Result;
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserForDetailedDto>(okResult.Value);
            Assert.Equal(user.UserName, returnValue.Username);
        }

        [Fact]
        public void GetUser_WhenCalled_ReturnsCurrentUser()
        {
            // Arrange
            var user = _recipeRepoMock.Setup(x => x.GetUser(2)).ReturnsAsync(new User{
                Id = 2,
                UserName = "josh1"
            });

            // Act
            var result = _usersController.GetUser().Result;
            

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetUsers_WhenCalled_ReturnsListOfUsers()
        {
            // Arrange
            var users = GetFakeUserList().ToList();
            var pageParams = new PageParams();
            var usersToPagedList = new PagedList<User>(users, 3, 1, 10);
            

            _recipeRepoMock.Setup(x => x.GetUsers(pageParams))
                .ReturnsAsync(usersToPagedList);

            // Act
            var result = _usersController.GetUsers(pageParams).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<UserForListDto>>(okResult.Value);
            // Assert.Equal(returnValue[0].Username, users[0].UserName);
            Assert.Equal(users.Count, returnValue.Count);
        }

        // [Fact]
        // public void UpdateEmail_UnauthorizedUserClaims_ReturnsUnauthorized()
        // {
        //     // arrange
        //     var userId = 1;
        //     var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);

        //     _recipeRepoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);

        //     // Act
        //     var result = _usersController.UpdateUserEmail(userId, new UserEmailForUpdateDto {
        //         Email = "josh2@famemail.com"
        //     }).Result;


        //     // assert
        //     Assert.IsType<UnauthorizedResult>(result);
            
        // }
        
        // [Fact]
        // public void UpdateEmail_UserNotFound_ReturnsNotFound()
        // {
        //     // arrange
        //     var userId = 2;
        //     var email = "josh2@famemail.com";
        //     var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);

        //     _recipeRepoMock.Setup(x => x.GetUser(userId));
        //     _authRepoMock.Setup(x => x.UserExists(email)).ReturnsAsync(true);

        //     // Act
        //     var result = _usersController.UpdateUserEmail(userId, new UserEmailForUpdateDto {
        //         Email = email
        //     }).Result;


        //     // assert
        //     Assert.IsType<NotFoundResult>(result);
        // }
        
        // [Fact]
        // public void UpdateEmail_EmailAlreadyInUse_ReturnsBadRequest()
        // {
        //     // arrange
        //     var userId = 2;
        //     var email = "josh2@famemail.com";
        //     var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);

        //     _recipeRepoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
        //     _authRepoMock.Setup(x => x.UserExists(email)).ReturnsAsync(true);

        //     // Act
        //     var result = _usersController.UpdateUserEmail(userId, new UserEmailForUpdateDto {
        //         Email = email
        //     }).Result;


        //     // assert
        //     var okResult = Assert.IsType<BadRequestObjectResult>(result);
        //     Assert.Equal("Email already in use", okResult.Value);
        // }
        
        // [Fact]
        // public void UpdateEmail_Successful_ReturnsSuccessMessage()
        // {
        //     // arrange
        //     var userId = 2;
        //     var email = "josh2@famemail.com";
        //     var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);

        //     _recipeRepoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
        //     _authRepoMock.Setup(x => x.UserExists(email)).ReturnsAsync(false);
        //     _recipeRepoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

        //     // Act
        //     var result = _usersController.UpdateUserEmail(userId, new UserEmailForUpdateDto {
        //         Email = email
        //     }).Result;


        //     // assert
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     Assert.Equal("Email address updated successfully", okResult.Value);
        // }
        
        // [Fact]
        // public void UpdateEmail_FailsOnSave_ReturnsBadRequest()
        // {
        //     // arrange
        //     var userId = 2;
        //     var email = "josh2@famemail.com";
        //     var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);

        //     _recipeRepoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
        //     _authRepoMock.Setup(x => x.UserExists(email)).ReturnsAsync(false);
        //     _recipeRepoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

        //     // Act
        //     var result = _usersController.UpdateUserEmail(userId, new UserEmailForUpdateDto {
        //         Email = email
        //     }).Result;


        //     // assert
        //     var okResult = Assert.IsType<BadRequestObjectResult>(result);
        //     Assert.Equal("Updating email address failed on save", okResult.Value);
        // }

        private ICollection<User> GetFakeUserList()
        {
            return new List<User>()
            {
                new User()
                {
                    Id = 1,
                    UserName = "Bob"
                },
                new User()
                {
                    Id = 2,
                    UserName = "George"
                },
                new User()
                {
                    Id = 3,
                    UserName = "Susie"
                }
            };
        }

    }
}