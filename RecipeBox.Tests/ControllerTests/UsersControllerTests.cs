using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RecipeBox.API.Controllers;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;
using Xunit;

namespace RecipeBox.Tests.ControllerTests
{
    public class UsersControllerTests
    {
        private Mock<IRecipeRepository> _repoMock;
        private UsersController _usersController;
        private readonly ClaimsPrincipal _userClaims;

        public UsersControllerTests()
        {
            _repoMock = new Mock<IRecipeRepository>();

            var mockMapper = new MapperConfiguration(cfg => {cfg.AddProfile(
                new AutoMapperProfiles()); });

            var mapper = mockMapper.CreateMapper();

            _usersController = new UsersController(_repoMock.Object, mapper);

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
            var user = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);
            
            _repoMock.Setup(x => x.GetUser(userId))
                .ReturnsAsync(user);

            // Act
            var result = _usersController.GetUser(userId).Result;
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserForDetailedDto>(okResult.Value);
            Assert.Equal(user.Username, returnValue.Username);
        }

        [Fact]
        public void GetUsers_WhenCalled_ReturnsListOfUsers()
        {
            // Arrange
            var users = GetFakeUserList().ToList();

            _repoMock.Setup(x => x.GetUsers())
                .ReturnsAsync(users);

            // Act
            var result = _usersController.GetUsers().Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<UserForListDto>>(okResult.Value);
            Assert.Equal(returnValue[0].Username, users[0].Username);
            Assert.Equal(users.Count, returnValue.Count);
        }

        private ICollection<User> GetFakeUserList()
        {
            return new List<User>()
            {
                new User()
                {
                    UserId = 1,
                    Username = "Bob"
                },
                new User()
                {
                    UserId = 2,
                    Username = "George"
                },
                new User()
                {
                    UserId = 3,
                    Username = "Susie"
                }
            };
        }

    }
}