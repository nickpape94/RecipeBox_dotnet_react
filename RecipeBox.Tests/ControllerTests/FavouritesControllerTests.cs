using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RecipeBox.API.Controllers;
using RecipeBox.API.Data;
using RecipeBox.API.Models;
using Xunit;

namespace RecipeBox.Tests.ControllerTests
{
    public class FavouritesControllerTests
    {
        private Mock<IRecipeRepository> _repoMock;
        private FavouritesController _favouritesController;
        private readonly ClaimsPrincipal _userClaims;

        public FavouritesControllerTests()
        {
            _repoMock = new Mock<IRecipeRepository>();

            _favouritesController = new FavouritesController(_repoMock.Object);

            // Mock claims types
            _userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "josh"),
                new Claim(ClaimTypes.NameIdentifier, "2"),
            }, "mock"));
            _favouritesController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _userClaims }
            };
        }

        [Fact]
        public void GetUserFavourites()
        {
            // Arrange 
            int userId = 1;
            var favouritesFromRepo = GetNicksFavourites().Where(x => x.FavouriterId == userId);
            var postsFromRepo = GetPosts();

            _repoMock.Setup(x => x.GetFavourites(userId)).ReturnsAsync(favouritesFromRepo);
            _repoMock.Setup(x => x.GetPosts()).ReturnsAsync(postsFromRepo);

            // Act
            var result = _favouritesController.GetFavourites(userId).Result;
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void AddPostToFavourites_FalseUserClaims_ReturnsUnauthorized()
        {
            int userId = 1;
            var userFromRepo = GetUsers().SingleOrDefault(x => x.UserId == userId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);

            var result = _favouritesController.AddToFavourites(userId, 2).Result;

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void AddPostToFavourites_PostNotFound_ReturnsNotFound()
        {
            int userId = 2;
            int postId = 100;
            var userFromRepo = GetUsers().SingleOrDefault(x => x.UserId == userId);
            var postsFromRepo = GetPosts().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postsFromRepo);

            var result = _favouritesController.AddToFavourites(userId, postId).Result;

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void AddPostToFavourites_RecipeAlreadyFavourited_ReturnsBadRequest()
        {
            int userId = 2;
            int postId = 1;
            var userFromRepo = GetUsers().SingleOrDefault(x => x.UserId == userId);
            var postsFromRepo = GetPosts().SingleOrDefault(x => x.PostId == postId);
            var favouritesFromRepo = GetNicksFavourites().Where(x => x.FavouriterId == userId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postsFromRepo);
            _repoMock.Setup(x => x.GetFavourites(userId)).ReturnsAsync(favouritesFromRepo);

            var result = _favouritesController.AddToFavourites(userId, postId).Result;

            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Recipe has already been favourited", okResult.Value);
        }

        [Fact]
        public void AddPostToFavourites_Success_ReturnsOkObjectResult()
        {
            int userId = 2;
            int postId = 4;
            var userFromRepo = GetUsers().SingleOrDefault(x => x.UserId == userId);
            var postsFromRepo = GetPosts().SingleOrDefault(x => x.PostId == postId);
            var favouritesFromRepo = GetNicksFavourites().Where(x => x.FavouriterId == userId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postsFromRepo);
            _repoMock.Setup(x => x.GetFavourites(userId)).ReturnsAsync(favouritesFromRepo);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            var result = _favouritesController.AddToFavourites(userId, postId).Result;

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Recipe added successfully", okResult.Value);
        }

        [Fact]
        public async void AddPostToFavourites_FailsOnSave_ReturnsNewException()
        {
            int userId = 2;
            int postId = 4;
            var userFromRepo = GetUsers().SingleOrDefault(x => x.UserId == userId);
            var postsFromRepo = GetPosts().SingleOrDefault(x => x.PostId == postId);
            var favouritesFromRepo = GetNicksFavourites().Where(x => x.FavouriterId == userId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postsFromRepo);
            _repoMock.Setup(x => x.GetFavourites(userId)).ReturnsAsync(favouritesFromRepo);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            Exception ex = await Assert.ThrowsAsync<Exception>(() => _favouritesController.AddToFavourites(userId, postId));

            Assert.Equal("Adding the post to favourites failed on save", ex.Message);
        }

        [Fact]
        public void DeleteAFavourite_Unauthorized_ReturnsUnauthorized()
        {
            int userId = 1;
            int postId = 1;
            var userFromRepo = GetUsers().SingleOrDefault(x => x.UserId == userId);
            var postFromRepo = GetPosts().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);

            var result = _favouritesController.DeleteAFavourite(userId, postId).Result;

            Assert.IsType<UnauthorizedResult>(result);
        }
        
        [Fact]
        public void DeleteAFavourite_FavouriteNotFound_ReturnsNotFound()
        {
            int userId = 2;
            int postId = 100;
            var userFromRepo = GetUsers().SingleOrDefault(x => x.UserId == userId);
            var favouritesFromRepo = GetNicksFavourites();
            var postFromRepo = GetPosts().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(x => x.GetFavourites(userId)).ReturnsAsync(favouritesFromRepo);

            var result = _favouritesController.DeleteAFavourite(userId, postId).Result;

            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Recipe with id {postId} not found in favourites", okResult.Value);
        }
        
        [Fact]
        public void DeleteAFavourite_FavouriterIdUserIdMismatch_ReturnsUnAuthorized()
        {
            int userId = 2;
            int postId = 1;
            var userFromRepo = GetUsers().SingleOrDefault(x => x.UserId == userId);
            var favouritesFromRepo = GetNicksFavourites();

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetFavourites(userId)).ReturnsAsync(favouritesFromRepo);

            var result = _favouritesController.DeleteAFavourite(userId, postId).Result;

            var okResult = Assert.IsType<UnauthorizedResult>(result);
        }
        
        [Fact]
        public void DeleteAFavourite_Successful_ReturnssOk()
        {
            int userId = 2;
            int postId = 1;
            var userFromRepo = GetUsers().SingleOrDefault(x => x.UserId == userId);
            var favouritesFromRepo = GetJoshFavourites();

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetFavourites(userId)).ReturnsAsync(favouritesFromRepo);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            var result = _favouritesController.DeleteAFavourite(userId, postId).Result;

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Favourite successfully deleted", okResult.Value);
        }
        
        [Fact]
        public void DeleteAFavourite_FailsOnSave_ReturnssOk()
        {
            int userId = 2;
            int postId = 1;
            var userFromRepo = GetUsers().SingleOrDefault(x => x.UserId == userId);
            var favouritesFromRepo = GetJoshFavourites();

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetFavourites(userId)).ReturnsAsync(favouritesFromRepo);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            var result = _favouritesController.DeleteAFavourite(userId, postId).Result;

            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Could not delete photo from favourites", okResult.Value);
        }


        private ICollection<User> GetUsers()
        {
            return new List<User>()
            {
                new User()
                {
                    UserId = 1,
                    Username = "nick",
                    Favourites = GetNicksFavourites()
                    
                },
                new User()
                {
                    UserId = 2,
                    Username = "josh",
                    Favourites = GetJoshFavourites()
                }
            };
        }

        private ICollection<Post> GetPosts()
        {
            return new List<Post>()
            {
                new Post()
                {
                    PostId = 1,
                    NameOfDish = "scampi",
                    Description = "fried fish",
                    UserId = 1
                },
                new Post()
                {
                    PostId = 2,
                    NameOfDish = "steak",
                    Description = "fried fish",
                    UserId = 1
                },
                new Post()
                {
                    PostId = 3,
                    NameOfDish = "Chocolate",
                    Description = "tasty chocolate",
                    UserId = 2
                },
                new Post()
                {
                    PostId = 4,
                    NameOfDish = "Beef wellington",
                    Description = "beef",
                    UserId = 2
                }
            };
        }

        private ICollection<Favourite> GetNicksFavourites()
        {
            return new List<Favourite>()
            {

                new Favourite()
                {
                    Id = 1,
                    FavouriterId = 1,
                    PostId = 1,
                },
                new Favourite()
                {
                    Id = 2,
                    FavouriterId = 1,
                    PostId = 3,
                },
                // new Favourite()
                // {
                //     Id = 3,
                //     FavouriterId = 2,
                //     PostId = 1
                // },
                // new Favourite()
                // {
                //     Id = 4,
                //     FavouriterId = 2,
                //     PostId = 2
                // }
                
            };
        }
        private ICollection<Favourite> GetJoshFavourites()
        {
            return new List<Favourite>()
            {

                new Favourite()
                {
                    Id = 1,
                    FavouriterId = 2,
                    PostId = 1,
                },
                new Favourite()
                {
                    Id = 2,
                    FavouriterId = 2,
                    PostId = 3,
                }
                
            };
        }
    }
}