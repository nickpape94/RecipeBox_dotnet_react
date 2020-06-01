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
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;
using Xunit;

namespace RecipeBox.Tests
{
    public class PostsControllerTests
    {
        private Mock<IRecipeRepository> _repoMock;
        private PostsController _postsController;
        private readonly ClaimsPrincipal _userClaims;
        public PostsControllerTests()
        {
            _repoMock = new Mock<IRecipeRepository>();

            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfiles()); });

            var mapper = mockMapper.CreateMapper();

            _postsController = new PostsController(_repoMock.Object, mapper);

            // Mock claim types
            _userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "josh"),
                new Claim(ClaimTypes.NameIdentifier, "2"),
            }, "mock"));
            _postsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _userClaims }
            };
        }

        [Fact]
        public void GetPost_WhenCalled_ReturnsRightPost()
        {
            // Arrange
            var post = GetFakePostList().SingleOrDefault(x => x.PostId == 1);
            _repoMock.Setup(repo => repo.GetPost(1)).ReturnsAsync(post);
            
            // Act
            var result = _postsController.GetPost(1).Result;
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PostsForDetailedDto>(okResult.Value);
            Assert.Equal(post.NameOfDish, returnValue.NameOfDish);
        }
        
        [Fact]
        public void GetPosts_WhenCalled_ReturnsListOfPosts()
        {
            // Arrange
            var posts = GetFakePostList().ToList();
            _repoMock.Setup(x => x.GetPosts()).ReturnsAsync(posts);
            
            // Act
            var result = _postsController.GetPosts().Result;
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PostsForListDto>>(okResult.Value);
            Assert.Equal(posts.Count, returnValue.Count);
        }

        [Fact]
        public void CreatePost_UnauthorizedUserClaims_ReturnsUnauthorized()
        {
            // Arrange
            int userId = 1;
            var postForCreation = new PostForCreationDto
            {
                NameOfDish = "Katsu curry",
                Description = "chicken and rice",
                Ingredients = "chicken, rice",
                Method = "fry chicken, boil rice",
                PrepTime = "20 min",
                CookingTime = "20 min",
                Feeds = "3",
                Cuisine = "Japanese"
            };

            
            // Act
            var result = _postsController.CreatePost(userId, postForCreation).Result;

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
        
        // [Fact]
        // public void CreatePost_AuthorizedUser_CreatesAndReturnsNewPost()
        // {
        //     // Arrange
        //     int userId = 2;
        //     var postForCreation = new PostForCreationDto()
        //     {
        //         NameOfDish = "Katsu curry",
        //         Description = "chicken and rice",
        //         Ingredients = "chicken, rice",
        //         Method = "fry chicken, boil rice",
        //         PrepTime = "20 min",
        //         CookingTime = "20 min",
        //         Feeds = "3",
        //         Cuisine = "Japanese",
        //         UserId = userId
        //     };

        //     _repoMock.Setup(repo => repo.SaveAll()).ReturnsAsync(true);

             
        //     var user = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);
        //     // _repo.Setup(repo => repo.GetUser(userId))
        //     //     .ReturnsAsync(Task.FromResult<User>(user.Posts.Add(postForCreation)));

        //     // _repo.Setup(repo => repo.Add(user.Posts.Add(postForCreation)))
        //     //     .Returns(Task.FromResult<User>)
        //     //     .Verifiable();

        //     // Act
        //     var result = _postsController.CreatePost(userId, postForCreation).Result;

        //     // Assert
        //     Assert.IsType<PostsForDetailedDto>(result);
        // }

        private ICollection<Post> GetFakePostList()
        {
            return new List<Post>()
            {
                new Post()
                {
                    PostId = 1,
                    NameOfDish = "Lasagne",
                    Description = "Yum",
                    Ingredients = "Ragu, pasta, ricotta",
                    Method = "Cook",
                    PrepTime = "15 min",
                    CookingTime = "15 min",
                    Feeds = "3-4",
                    Cuisine = "Italian",
                    UserId = 1
                    
                },
                new Post()
                {
                    PostId = 2,
                    NameOfDish = "Risotto",
                    Description = "Yum",
                    Ingredients = "Rice, Parmesan",
                    Method = "Cook",
                    PrepTime = "25 min",
                    CookingTime = "25 min",
                    Feeds = "3-4",
                    Cuisine = "Italian",
                    UserId = 2

                }
            };
        }
        
        private ICollection<User> GetFakeUserList()
        {
            return new List<User>()
            {
                new User()
                {
                    UserId = 1,
                    Username = "mike",
                },
                new User()
                {
                    UserId = 2,
                    Username = "josh",
                }
            };
        }
    }
}