using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using RecipeBox.API.Controllers;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos.PhotoDtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;
using Xunit;

namespace RecipeBox.Tests.ControllerTests
{
    public class PostPhotosControllerTests
    {
        private Mock<IRecipeRepository> _repoMock;
        private Mock<IFormFile> _fileMock;
        private Mock<HttpClient> _handler;
        private PostPhotosController _photosController;
        private readonly ClaimsPrincipal _userClaims;

        public PostPhotosControllerTests()
        {
            _repoMock = new Mock<IRecipeRepository>();

            _fileMock = new Mock<IFormFile>();

            _handler = new Mock<HttpClient>();
            
            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfiles()); });

            var mapper = mockMapper.CreateMapper();

            var settings = new CloudinarySettings()
            {
                ApiKey = "A",
                ApiSecret = "B",
                CloudName = "C"
            };

            var someOptions = Options.Create(settings);

            _photosController = new PostPhotosController(_repoMock.Object, mapper, someOptions);

            _userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "josh"),
                new Claim(ClaimTypes.NameIdentifier, "2")
            }, "mock"));
            _photosController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = _userClaims }
            };
        }

        [Fact]
        public void GetPhoto_WhenPhotoExists_ReturnsRightPhoto()
        {
            // Arrange
            var postPhotoId = 1;
            var photoFromRepo = GetFakePostPhotoList().SingleOrDefault(x => x.PostPhotoId == postPhotoId);

            _repoMock.Setup(x => x.GetPostPhoto(postPhotoId)).ReturnsAsync(photoFromRepo);

            // Act
            var result = _photosController.GetPostPhoto(postPhotoId).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PostPhotosForReturnDto>(okResult.Value);
            Assert.True(returnValue.IsMain);
            Assert.Equal("My first photo!", returnValue.Description);
            
        }

        [Fact]
        public void SetMainPhoto_UnAuthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var userId = 1;
            var photoId = 1;
            var postId = 1;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);

            // Act
            var result = _photosController.SetMainPhoto(userId, photoId).Result;

            // Assert
            var OkResult = Assert.IsType<UnauthorizedResult>(result);
             
        }

        [Fact]
        public void SetMainPhoto_NoPhotoFound_ReturnsUnauthorized()
        {
            // Arrange
            var userId = 2;
            var photoId = 10;
            var postId = 2;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);

            // Act
            var result = _photosController.SetMainPhoto(userId, photoId).Result;

            // Assert
            var OkResult = Assert.IsType<UnauthorizedResult>(result);
             
        }

        [Fact]
        public void SetMainPhoto_PhotoAlreadyMain_ReturnsBadRequest()
        {
            // Arrange
            var postId = 2;
            var postPhotoId = 1;
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            var photoFromRepo = GetFakePostPhotoList().SingleOrDefault(x => x.PostPhotoId == postPhotoId);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);
            _repoMock.Setup(x => x.GetPostPhoto(postPhotoId)).ReturnsAsync(photoFromRepo);

            // Act
            var result = _photosController.SetMainPhoto(postId, postPhotoId).Result;

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This is already the main photo", okResult.Value);

             
        }
        
        [Fact]
        public void SetMainPhoto_FailsOnSave_ReturnsBadRequest()
        {
            // Arrange
            var postId = 2;
            var postPhotoId = 2;
            var userFromRepo = GetFakePostList().SingleOrDefault(x => x.UserId == postId);
            var photoFromRepo = GetFakePostPhotoList().SingleOrDefault(x => x.PostPhotoId == postPhotoId);
            var currentMainPhoto = GetFakePostPhotoList().SingleOrDefault(x => x.IsMain);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPostPhoto(postPhotoId)).ReturnsAsync(photoFromRepo);
            _repoMock.Setup(x => x.GetMainPhotoForPost(postId)).ReturnsAsync(currentMainPhoto);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            var result = _photosController.SetMainPhoto(postId, postPhotoId).Result;

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Could not set photo to main", okResult.Value);

        }
        
        [Fact]
        public void SetMainPhoto_Successful_ReturnsNoContent()
        {
            // Arrange
            var postId = 2;
            var postPhotoId = 2;
            var userFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            var photoFromRepo = GetFakePostPhotoList().SingleOrDefault(x => x.PostPhotoId == postPhotoId);
            var currentMainPhoto = GetFakePostPhotoList().SingleOrDefault(x => x.IsMain);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPostPhoto(postPhotoId)).ReturnsAsync(photoFromRepo);
            _repoMock.Setup(x => x.GetMainPhotoForPost(postId)).ReturnsAsync(currentMainPhoto);
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _photosController.SetMainPhoto(postId, postPhotoId).Result;

            // Assert
            var okResult = Assert.IsType<NoContentResult>(result);

        }

        [Fact]
        public void FindPhoto_PublicId_IsNull_DeletePhoto_Successful_ReturnsOk()
        {
            // Arrange
            int postId = 2;
            int postPhotoId = 2;
            var photoFromRepo = GetFakePostPhotoList().SingleOrDefault(x => x.PostPhotoId == postPhotoId);
            var userFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPostPhoto(postPhotoId)).ReturnsAsync(photoFromRepo);
            _repoMock.Setup(x => x.Delete(photoFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _photosController.DeletePhoto(postId, postPhotoId).Result as OkResult;

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(new OkObjectResult(okResult).StatusCode, result.StatusCode);

        }
        
        [Fact]
        public void FindPhoto_PublicId_NotNull_DeletePhoto_Successful_ReturnsOk()
        {
            // Arrange
            int postId = 2;
            int postPhotoId = 3;
            var photoFromRepo = GetFakePostPhotoList().SingleOrDefault(x => x.PostPhotoId == postPhotoId);
            var userFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);
            
            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPostPhoto(postPhotoId)).ReturnsAsync(photoFromRepo);
            // _handler.Protected()
            //     .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            //     .ReturnsAsync(new HttpResponseMessage {
            //         StatusCode = HttpStatusCode.OK,
            //         Content = new StringContent("ok")
            //     });

            // _handler.Setup(client => client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>())).ReturnsAsync(_mockResponse.Object);
            _repoMock.Setup(x => x.Delete(photoFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            // _photosController.ControllerContext = this.RequestWithFile();
            // result.Result is null. Fix!
            var result = _photosController.DeletePhoto(postId, postPhotoId).Result as OkResult;

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(new OkObjectResult(okResult).StatusCode, result.StatusCode);

        }
        
        [Fact]
        public void FindPhoto_DeletionFailsOnSave_ReturnsBadRequest()
        {
            // Arrange
            int postId = 2;
            int postPhotoId = 2;
            var photoFromRepo = GetFakePostPhotoList().SingleOrDefault(x => x.PostPhotoId == postPhotoId);
            var userFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPostPhoto(postPhotoId)).ReturnsAsync(photoFromRepo);
            _repoMock.Setup(x => x.Delete(photoFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            var result = _photosController.DeletePhoto(postId, postPhotoId).Result as BadRequestObjectResult;

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to delete the photo", okResult.Value);

        }
        
        [Fact]
        public void FindPhoto_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            int postId = 1;
            int postPhotoId = 2;
            var photoFromRepo = GetFakePostPhotoList().SingleOrDefault(x => x.PostPhotoId == postPhotoId);
            var userFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetPostPhoto(postPhotoId)).ReturnsAsync(photoFromRepo);
            _repoMock.Setup(x => x.Delete(photoFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            var result = _photosController.DeletePhoto(postId, postPhotoId).Result;

            // Assert
            var okResult = Assert.IsType<UnauthorizedResult>(result);

        }

        [Fact]
        public void Delete_Post_And_All_Associated_Cloudinary_Images_Unauthorized_User_Claims()
        {
            int userId = 1;
            int postId = 1;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);

            var result = _photosController.DeletePost(userId, postId).Result;

            var okResult = Assert.IsType<UnauthorizedResult>(result);
        }
       
        [Fact]
        public void Delete_Post_And_All_Associated_Cloudinary_Images_Authorized()
        {
            // Arrange
            int userId = 2;
            int postId = 2;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);

            foreach(var photo in postFromRepo.PostPhoto) 
            {
                _repoMock.Setup(x => x.GetPostPhoto(photo.PostPhotoId)).ReturnsAsync(GetFakePostPhotoList().SingleOrDefault(x => x.PostPhotoId == photo.PostPhotoId));
            }

            _repoMock.Setup(x => x.Delete(postFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _photosController.DeletePost(userId, postId).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully deleted post", okResult.Value);
        }
        
        [Fact]
        public async void Delete_Post_And_All_Associated_Cloudinary_Images_FailsOnSave()
        {
            // Arrange
            int userId = 2;
            int postId = 2;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);

            foreach(var photo in postFromRepo.PostPhoto) 
            {
                _repoMock.Setup(x => x.GetPostPhoto(photo.PostPhotoId)).ReturnsAsync(GetFakePostPhotoList().SingleOrDefault(x => x.PostPhotoId == photo.PostPhotoId));
            }

            _repoMock.Setup(x => x.Delete(postFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            Exception result = await Assert.ThrowsAsync<Exception>(() => _photosController.DeletePost(userId, postId));

            // Assert
            
            Assert.Equal($"Deleting post {postId} failed on save", result.Message);
        }
        
        [Fact]
        public  void Delete_Post_And_All_Associated_Cloudinary_Images_Unauthorized_User()
        {
            // Arrange
            int userId = 2;
            int postId = 1;
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);
            var postFromRepo = GetFakePostList().SingleOrDefault(x => x.PostId == postId);

            _repoMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postFromRepo);

            // Act
            var result = _photosController.DeletePost(userId, postId).Result;

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        private ICollection<Post> GetFakePostList()
        {

            return new List<Post>()
            {
                
                new Post()
                {
                    PostId = 1,
                    UserId = 1,
                    NameOfDish = "cake",
                    Ingredients = "flour, sugar, chocolate",
                    
                },
                new Post()
                {
                    PostId = 2,
                    UserId = 2,
                    NameOfDish = "bbq",
                    Cuisine = "american",
                    PostPhoto = GetFakePostPhotoList()
                    
                }
            };
        }

        private ICollection<PostPhoto> GetFakePostPhotoList()
        {

            return new List<PostPhoto>()
            {
                
                new PostPhoto()
                {
                    PostId = 2,
                    PostPhotoId = 1,
                    Url = "http://res.cloudinary.com/deszzup5p/image/upload/v1592234384/RecipeApp/user_photos/z0ki54dbqgqbobwc6qpb.jpg",
                    IsMain = true,
                    PublicId = "RecipeApp/user_photos/z0ki54dbqgqbobwc6qpb",
                    Description = "My first photo!",
                    
                    
                    
                },
                new PostPhoto()
                {
                    PostId = 2,
                    PostPhotoId = 2,
                    Url = "http://res.cloudinary.com/deszzup5p/image/upload/v1592234485/RecipeApp/user_photos/z5y795px3ac8afjawanb.jpg",
                    IsMain = false,
                    PublicId = null
                    
                },
                new PostPhoto()
                {
                    PostId = 2,
                    PostPhotoId = 3,
                    Url = "https://icatcare.org/app/uploads/2018/07/Thinking-of-getting-a-cat.png",
                    IsMain = false,
                    PublicId = "123456"
                    
                }
            };
        }

        private ICollection<User> GetFakeUserList()
        {

            return new List<User>()
            {
                
                new User()
                {
                    Id = 1,
                    UserName = "mike",
                    
                },
                new User()
                {
                    Id = 2,
                    UserName = "josh",
                    
                }
            };
        }

        private ControllerContext RequestWithFile()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("Content-Type", "multipart/form-data");
            var file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file")), 0, 0, "Data", "dummy.txt");
            httpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { file });
            var actx = new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor());
            return new ControllerContext(actx);
        }
    }
}