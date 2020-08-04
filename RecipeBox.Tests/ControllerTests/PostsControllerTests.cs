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
        public void UpdatePost_UserIdOfPostMismatch_ReturnsUnauthorized()
        {
            // Arrange
            int userId = 2;
            int postId = 3;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);
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

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            
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
        public void DeletingPost_UnauthorizedUserClaims_ReturnsUnauthorized()
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
        public void DeletingPost_UserIdOfPostMismatch_ReturnsUnauthorized()
        {
            // Arrange
            int userId = 2;
            int postId = 3;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            
            _repoMock.Setup(p => p.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(p => p.Delete(postFromRepo));
            _repoMock.Setup(s => s.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.DeletePost(userId, postId).Result;

            // Assert
            var okResult = Assert.IsType<UnauthorizedResult>(result);
            

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
        public void AddComment_PostNotFound_ReturnsNotFound()
        {
            // Arrange
            int userId = 2;
            int postId = 5;
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
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Post with id 5 not found", okResult.Value);
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

        [Fact]
        public void DeleteComment_NotAuthorized_ReturnsUnauthorized()
        {
            // Arrange
            int userId = 1;
            int commentId = 2;
            var commentFromRepo = GetFakeCommentsList().SingleOrDefault(x => x.CommenterId == userId);

            _repoMock.Setup(x => x.GetComment(commentId)).ReturnsAsync(commentFromRepo);
            _repoMock.Setup(x => x.Delete(commentFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.DeleteComment(commentId, userId).Result;

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
            
        }

        [Fact]
        public async void DeleteComment_FailsOnSave_ReturnsException()
        {
            // Arrange
            int userId = 2;
            int commentId = 2;
            var commentFromRepo = GetFakeCommentsList().SingleOrDefault(x => x.CommenterId == userId);

            _repoMock.Setup(x => x.GetComment(commentId)).ReturnsAsync(commentFromRepo);
            _repoMock.Setup(x => x.Delete(commentFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            Exception ex = await Assert.ThrowsAsync<Exception>(() => _postsController.DeleteComment(commentId, userId));

            // Assert
            Assert.Equal("Deleting the comment failed on save", ex.Message);

        }

        [Fact]
        public void DeleteComment_Authorized_SuccessfullyDeletesPost()
        {
            // Arrange
            int userId = 2;
            int commentId = 2;
            var commentFromRepo = GetFakeCommentsList().SingleOrDefault(x => x.CommenterId == userId);

            _repoMock.Setup(x => x.GetComment(commentId)).ReturnsAsync(commentFromRepo);
            _repoMock.Setup(x => x.Delete(commentFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.DeleteComment(commentId, userId).Result as OkObjectResult;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Comment was successfully deleted", okResult.Value);
        }

        [Fact]
        public void DeleteComment_CommentNotFound_ReturnsNotFound()
        {
            // Arrange
            int userId = 2;
            int commentId = 12;
            var commentFromRepo = GetFakeCommentsList().SingleOrDefault(x => x.CommenterId == commentId);

            _repoMock.Setup(x => x.GetComment(commentId)).ReturnsAsync(commentFromRepo);
            _repoMock.Setup(x => x.Delete(commentFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.DeleteComment(commentId, userId).Result;

            // Assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Comment 12 not found", okResult.Value);
        }

        [Fact]
        public void DeleteComment_CommentIdUserIdMismatch_ReturnsUnauthorized()
        {
            // Arrange
            int userId = 2;
            int commentId = 1;
            var commentFromRepo = GetFakeCommentsList().SingleOrDefault(x => x.CommenterId == commentId);

            _repoMock.Setup(x => x.GetComment(commentId)).ReturnsAsync(commentFromRepo);
            _repoMock.Setup(x => x.Delete(commentFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.DeleteComment(commentId, userId).Result;

            // Assert
            var okResult = Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void UpdateComment_UserClaimsUnauthorized_ReturnsUnauthorized()
        {
            // Arrange
            int commentId = 2;
            int userId = 1;
            var commentForUpdate = new CommentForUpdateDto
            {
                Text = "My newly updated comment"
            };
            var commentFromRepo = GetFakeCommentsList().SingleOrDefault(x => x.CommentId == commentId);

            _repoMock.Setup(x => x.GetComment(commentId)).ReturnsAsync(commentFromRepo);
            _repoMock.Setup(x => x.Update(commentFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.UpdateComment(userId, commentId, commentForUpdate).Result;

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public void UpdateComment_CommentNotFound_ReturnsNotFound()
        {
            int commentId = 12;
            int userId = 2;
            var commentForUpdate = new CommentForUpdateDto
            {
                Text = "My newly updated comment"
            };
            var commentFromRepo = GetFakeCommentsList().SingleOrDefault(x => x.CommentId == commentId);
            // var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == commentFromRepo.PostId);

            _repoMock.Setup(x => x.GetComment(commentId)).ReturnsAsync(commentFromRepo);
            _repoMock.Setup(x => x.Update(commentFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.UpdateComment(userId, commentId, commentForUpdate).Result;

            // Assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Comment 12 not found", okResult.Value);

        }
        
        [Fact]
        public void UpdateComment_CommenterIdUserIdMismatch_ReturnsUnauthorized()
        {
            int commentId = 1;
            int userId = 2;
            var commentForUpdate = new CommentForUpdateDto
            {
                Text = "My newly updated comment"
            };
            var commentFromRepo = GetFakeCommentsList().SingleOrDefault(x => x.CommentId == commentId);
        

            _repoMock.Setup(x => x.GetComment(commentId)).ReturnsAsync(commentFromRepo);
            _repoMock.Setup(x => x.Update(commentFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.UpdateComment(userId, commentId, commentForUpdate).Result;

            // Assert
            var okResult = Assert.IsType<UnauthorizedResult>(result);

        }

        [Fact]
        public async void UpdateComment_FailsOnSave_ReturnsException()
        {
            // Arrange
            int commentId = 2;
            int userId = 2;
            var commentForUpdate = new CommentForUpdateDto
            {
                Text = "My newly updated comment"
            };
            var commentFromRepo = GetFakeCommentsList().SingleOrDefault(x => x.CommentId == commentId);

            _repoMock.Setup(x => x.GetComment(commentId)).ReturnsAsync(commentFromRepo);
            _repoMock.Setup(x => x.Update(commentFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            Exception ex = await Assert.ThrowsAsync<Exception>(() => _postsController.UpdateComment(userId, commentId, commentForUpdate));

            // Assert
            Assert.Equal("Updating the comment failed on save", ex.Message);
        }

        [Fact]
        public void UpdateComment_Authorized_SuccessfullyUpdatedComment()
        {
            int commentId = 2;
            int userId = 2;
            var commentForUpdate = new CommentForUpdateDto
            {
                Text = "My newly updated comment"
            };
            var commentFromRepo = GetFakeCommentsList().SingleOrDefault(x => x.CommentId == commentId);
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == commentFromRepo.PostId);

            _repoMock.Setup(x => x.GetComment(commentId)).ReturnsAsync(commentFromRepo);
            _repoMock.Setup(x => x.Update(commentFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);
            _repoMock.Setup(x => x.GetPost(commentFromRepo.PostId)).ReturnsAsync(postFromRepo);

            // Act
            var result = _postsController.UpdateComment(userId, commentId, commentForUpdate).Result as CreatedAtRouteResult;

            // Assert
            var okResult = Assert.IsType<CreatedAtRouteResult>(result);
            var updatedComment = Assert.IsType<PostsForDetailedDto>(result.Value);

        }

        [Fact]
        public void AddRating_Unauthorized_UserClaims_Return_Unauthorized()
        {
            // Arrange
            var userId = 1;
            var postId = 2;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);

            // Act
            var result = _postsController.AddRatingToPost(userId, postId, new RatePostDto{
                Score = 3,
                RaterId = userId
            }).Result;

            // Assert
            var okResult = Assert.IsType<UnauthorizedResult>(result);
        }
        
        [Fact]
        public void AddRating_User_Tries_To_Rate_Own_Post_Return_Unauthorized()
        {
            // Arrange
            var userId = 2;
            var postId = 2;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);

            // Act
            var result = _postsController.AddRatingToPost(userId, postId, new RatePostDto{
                Score = 3,
                RaterId = userId
            }).Result;

            // Assert
            var okResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("You cannot rate your own recipe", okResult.Value);
        }

        [Fact]
        public void AddRating_User_AlreadyRated_Post_UpdatesPost()
        {
            // Arrange
            var userId = 2;
            var postId = 3;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            var ratingFromRepo = GetFakeRatingsList().SingleOrDefault(x => x.RaterId == userId && x.PostId == postId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(x => x.GetRating(userId, postId)).ReturnsAsync(ratingFromRepo);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.AddRatingToPost(userId, postId, new RatePostDto{
                Score = 5,
                RaterId = userId
            }).Result;

            // Assert
            var okResult = Assert.IsType<CreatedAtRouteResult>(result);
            var returnPost = Assert.IsType<PostsForDetailedDto>(okResult.Value);
        }
        
        [Fact]
        public void AddRating_Post_NotRated_RatePost()
        {
            // Arrange
            var userId = 2;
            var postId = 4;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _postsController.AddRatingToPost(userId, postId, new RatePostDto{
                Score = 5,
                RaterId = userId
            }).Result;

            // Assert
            var okResult = Assert.IsType<CreatedAtRouteResult>(result);
            var returnPost = Assert.IsType<PostsForDetailedDto>(okResult.Value);
        }
        
        [Fact]
        public void AddRating_FailsOnSave_Returns_BadRequest()
        {
            // Arrange
            var userId = 2;
            var postId = 4;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.UserId == userId);
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            var result = _postsController.AddRatingToPost(userId, postId, new RatePostDto{
                Score = 5,
                RaterId = userId
            }).Result;

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to add a rating to post", okResult.Value);
        }

        [Fact]
        public void GetAverageRatingOfPosts_NoRatingsYet_Returns_Default()
        {
            // Arrange
            var postId = 2;
            var ratingsFromRepo = GetFakeRatingsList2().Where(x => x.PostId == postId);

            // Act
            _repoMock.Setup(x => x.GetRatings(postId)).ReturnsAsync(ratingsFromRepo);
            
            var result = _postsController.GetAverageRating(postId).Result;

            // Assert
            var okResult = Assert.IsType<double>(result);
            Assert.Equal(ratingsFromRepo.Count().ToString(), okResult.ToString());
        }
        
        [Fact]
        public void GetAverageRatingOfPosts_Returns_OkObject()
        {
            // Arrange
            var postId = 4;
            var ratingsFromRepo = GetFakeRatingsList2().Where(x => x.PostId == postId);

            // Act
            _repoMock.Setup(x => x.GetRatings(postId)).ReturnsAsync(ratingsFromRepo);
            
            var result = _postsController.GetAverageRating(postId).Result;

            // Assert
            var okResult = Assert.IsType<double>(result);
            // Assert.Equal("No ratings for this post yet", okResult.Value);
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

        private ICollection<Rating> GetFakeRatingsList()
        {
            return new List<Rating>()
            {
                new Rating()
                {
                    RatingId = 1,
                    Score = 4,
                    RaterId = 2,
                    PostId = 3
                },
                
            };
        }

        private ICollection<Rating> GetFakeRatingsList2()
        {
            return new List<Rating>()
            {
                new Rating()
                {
                    RatingId = 2,
                    Score = 3,
                    RaterId = 1,
                    PostId = 4
                },
                new Rating()
                {
                    RatingId = 3,
                    Score = 5,
                    RaterId = 3,
                    PostId = 4
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

                },
                new Post()
                {
                    PostId = 3,
                    NameOfDish = "Cake",
                    Description = "Birthday treat",
                    Ingredients = "sugar, flour",
                    Method = "Cook",
                    PrepTime = "25 min",
                    CookingTime = "25 min",
                    Feeds = "3-4",
                    Cuisine = "british",
                    UserId = 1,
                    Comments = null,
                    Ratings = GetFakeRatingsList()
                },
                new Post()
                {
                    PostId = 4,
                    NameOfDish = "Ragu",
                    Description = "Italian classic",
                    Ingredients = "spaghetti, mince",
                    Method = "Cook",
                    PrepTime = "25 min",
                    CookingTime = "25 min",
                    Feeds = "3-4",
                    Cuisine = "Italian",
                    UserId = 1,
                    Comments = null,
                    Ratings = GetFakeRatingsList2()
                    
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