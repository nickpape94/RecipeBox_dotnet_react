using System;
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
            var result = _postsController.CreatePost(userId, postForCreation)
                .Result;

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
        
        [Fact]
        public async void CreatePost_FailsOnSave_ReturnsException()
        {
            // Arrange
            int userId = 2;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);
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

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);
            
            // Act
            Exception ex = await Assert.ThrowsAsync<Exception>(() => _postsController.CreatePost(userId, postForCreation));

            // Assert
            Assert.Equal("Creating the post failed on save", ex.Message);
        }
        
        [Fact]
        public void CreatePost_UserAuthorized_ReturnsPost()
        {
            // Arrange
            int userId = 2;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);

            var postForCreation = new PostForCreationDto
            {
                NameOfDish = "Katsu curry",
                Description = "chicken and rice",
                Ingredients = "chicken, rice",
                Method = "fry chicken, boil rice",
                PrepTime = "20 min",
                CookingTime = "20 min",
                Feeds = "3",
                Cuisine = "Japanese",
                UserId = userId
            };

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);
            
            //Act
            var result = _postsController.CreatePost(userId, postForCreation)
                .Result;

            // Assert
            var okResult = Assert.IsType<CreatedAtRouteResult>(result);
            var returnValue = Assert.IsType<PostsForDetailedDto>(okResult.Value);
            
        }
        
        [Fact]
        public void UpdatePost_UnauthorizedUserClaims_ReturnsUnauthorized()
        {
            // Arrange
            int userId = 1;
            int postId = 1;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            var postForUpdate = new PostForUpdateDto()
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

            // _repoMock.Setup(r => r.GetPost(2)).R
            
            // Act
            var result = _postsController.UpdatePost(userId, postId, postForUpdate).Result;

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async void UpdatePost_SaveFails_ThrowsException()
        {
            // Arrange
            int userId = 2;
            int postId = 2;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            var postForUpdate = new PostForUpdateDto()
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
            
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(x => x.Update(postFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            Exception ex = await Assert.ThrowsAsync<Exception>(() => _postsController.UpdatePost(userId, postId, postForUpdate));

            // Assert
            Assert.Equal(ex.Message, $"Updating post {postId} failed on save");
        }

        [Fact]
        public void UpdatePost_Success_ReturnsUpdatedPost()
        {
            // Arrange
            int userId = 2;
            int postId = 2;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            var postForUpdate = new PostForUpdateDto()
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
            
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(x => x.Update(postForUpdate));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result =  _postsController.UpdatePost(userId, postId, postForUpdate).Result as CreatedAtRouteResult;
            // Assert
            var okResult = Assert.IsType<CreatedAtRouteResult>(result);
            var updatedPost = Assert.IsType<PostsForDetailedDto>(result.Value);
            Assert.Equal(postFromRepo.NameOfDish, updatedPost.NameOfDish);
            Assert.Equal(postFromRepo.Cuisine, updatedPost.Cuisine);
            Assert.Equal(postFromRepo.Description, updatedPost.Description);
        }

        [Fact]
        public void DeletingPost_Unauthorized_ReturnsUnauthorized()
        {
            // Arrange
            int userId = 1;
            int postId = 2;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            
            _repoMock.Setup(p => p.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(p => p.Delete(postFromRepo));
            _repoMock.Setup(s => s.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.DeletePost(userId, postId).Result as UnauthorizedResult;

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            
            // Assert.Equal(new OkObjectResult(okResult).StatusCode, result.StatusCode);

        }
        
        [Fact]
        public async void DeletingPost_FailsOnSave_ReturnsException()
        {
            // Arrange
            int userId = 2;
            int postId = 2;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            
            _repoMock.Setup(p => p.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(p => p.Delete(postFromRepo));
            _repoMock.Setup(s => s.SaveAll()).ReturnsAsync(false);

            // Act
            // var result = _postsController.DeletePost(userId, postId).Result as Exception;
            Exception ex = await Assert.ThrowsAsync<Exception>(() => _postsController.DeletePost(userId, postId));
            
            // Assert
            Assert.Equal(ex.Message, $"Deleting post {postId} failed on save");


        }
        
        [Fact]
        public void DeletingPost_Successfull_Returns201()
        {
            // Arrange
            int userId = 2;
            int postId = 2;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            
            _repoMock.Setup(p => p.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(p => p.Delete(postFromRepo));
            _repoMock.Setup(s => s.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.DeletePost(userId, postId).Result as OkObjectResult;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            Assert.Equal(new OkObjectResult(okResult).StatusCode, result.StatusCode);

        }

        [Fact]
        public void AddComment_Unauthorized_ReturnsUnauth()
        {
            // Arrange
            int userId = 1;
            int postId = 2;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            var commentForCreation = new CommentForCreationDto
            {
                Text = "Test comment2"
            };
            commentForCreation.CommenterId = userId;

        
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(x => x.Add(commentForCreation));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.AddComment(userId, postId, commentForCreation).Result;

            // Assert
            var okResult = Assert.IsType<UnauthorizedResult>(result);
            // var returnPost = Assert.IsType<PostsForDetailedDto>(okResult.Value);
            

        }

        [Fact]
        public async void AddComment_FailsOnSave_ThrowsException()
        {
            // Arrange
            int userId = 2;
            int postId = 2;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            var commentForCreation = new CommentForCreationDto
            {
                Text = "Test comment"
            };
            commentForCreation.CommenterId = userId;

        
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(x => x.Add(commentForCreation));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            Exception ex = await Assert.ThrowsAsync<Exception>(() => _postsController.AddComment(userId, postId, commentForCreation));

            // Assert
            Assert.Equal($"Creating the comment failed on save", ex.Message);
            
            
            
        }

        [Fact]
        public void AddComment_CommentAddedSuccessfully_ReturnsPostWithComment()
        {
            // Arrange
            int userId = 2;
            int postId = 2;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);
            var commentForCreation = new CommentForCreationDto
            {
                Text = "Test comment"
            };
            commentForCreation.CommenterId = userId;

        
            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(x => x.Add(commentForCreation));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.AddComment(userId, postId, commentForCreation).Result;

            // Assert
            var okResult = Assert.IsType<CreatedAtRouteResult>(result);
            var returnPost = Assert.IsType<PostsForDetailedDto>(okResult.Value);

        }

        private ICollection<Comment> GetFakeCommentsList()
        {
            return new List<Comment>()
            {
                new Comment()
                {
                    CommentId = 1,
                    Text = "comment 1",
                    CommenterId = 1,
                    PostId = 2
                },
                new Comment()
                {
                    CommentId = 2,
                    Text = "comment 2",
                    CommenterId = 2,
                    PostId = 2
                }

            };
        }

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
                    UserId = 2,
                    
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
                    UserId = 2,
                    Comments = GetFakeCommentsList()

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
                    Posts = GetFakePostList()
                    
                }
            };
        }
    }
}