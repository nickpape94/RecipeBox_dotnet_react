using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
    public class UserPhotosControllerTests
    {
        private Mock<IRecipeRepository> _repoMock;
        private Mock<IFormFile> _fileMock;
        private Mock<Cloudinary> _cloudinaryMock;
        private Mock<HttpClient> _handler;
        private UserPhotosController _photosController;
        private readonly ClaimsPrincipal _userClaims;

        public UserPhotosControllerTests()
        {
            _repoMock = new Mock<IRecipeRepository>();

            _fileMock = new Mock<IFormFile>();

            _cloudinaryMock = new Mock<Cloudinary>();

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

            _photosController = new UserPhotosController(_repoMock.Object, mapper, someOptions);

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
            var userPhotoId = 1;
            var photoFromRepo = GetFakeUserPhotoList().SingleOrDefault(x => x.UserPhotoId == userPhotoId);

            _repoMock.Setup(x => x.GetUserPhoto(userPhotoId)).ReturnsAsync(photoFromRepo);

            // Act
            var result = _photosController.GetUserPhoto(userPhotoId).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserPhotosForReturnDto>(okResult.Value);
            Assert.True(returnValue.IsMain);
            Assert.Equal("My first photo!", returnValue.Description);
            
        }

        [Fact]
        public void AddPhoto_Unauthorized_UserClaims_ReturnsUnauthorized()
        {
            var userId = 1;
            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(GetFakeUserList().FirstOrDefault(x => x.Id == userId));

            var content = "Hello World from a Fake File";
            var fileName = "test.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            _fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            _fileMock.Setup(_ => _.FileName).Returns(fileName);
            _fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = _fileMock.Object;

            var userPhotoForCreationDto = new UserPhotoForCreationDto()
            {
                Url = "example@random.com",
                PublicId = "randompic@cloudinary.com",
                File = file
            };

            var result = _photosController.AddPhotoForUser(userId, userPhotoForCreationDto).Result;

            Assert.IsType<UnauthorizedResult>(result);
        }
        
        // [Fact]
        // public void AddPhoto_DeleteCurrentPhotoIfExists_PublicIdNotNull()
        // {
        //     var userId = 2;
        //     _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(new User(){
        //         Id = userId,
        //         UserName = "josh",
        //         UserPhotos = new List<UserPhoto>(){
        //             new UserPhoto()
        //             {
        //                 UserId = 2,
        //                 UserPhotoId = 1,
        //                 Url = "http://res.cloudinary.com/deszzup5p/image/upload/v1592234384/RecipeApp/user_photos/z0ki54dbqgqbobwc6qpb.jpg",
        //                 IsMain = true,
        //                 PublicId = null,
        //                 Description = "My first photo!"
        //             }
        //         }
        //     });

        //     var content = "Hello World from a Fake File";
        //     var fileName = "test.pdf";
        //     var ms = new MemoryStream();
        //     var writer = new StreamWriter(ms);
        //     writer.Write(content);
        //     writer.Flush();
        //     ms.Position = 0;
        //     _fileMock.Setup(x => x.OpenReadStream()).Returns(ms);
        //     _fileMock.Setup(x => x.FileName).Returns(fileName);
        //     _fileMock.Setup(x => x.Length).Returns(ms.Length);
        //     // _cloudinaryMock.Setup(x => x.Upload(new ImageUploadParams(){

        //     // })).Ret

        //     var file = _fileMock.Object;

        //     var userPhotoForCreationDto = new UserPhotoForCreationDto()
        //     {
        //         Url = "example@random.com",
        //         PublicId = null,
        //         File = file
        //     };

        //     var result = _photosController.AddPhotoForUser(userId, userPhotoForCreationDto).Result;

        //     Assert.IsType<UnauthorizedResult>(result);
        // }

        [Fact]
        public void FindPhoto_PublicId_IsNull_DeletePhoto_Successful_ReturnsOk()
        {
            // Arrange
            int userId = 2;
            int userPhotoId = 2;
            var photoFromRepo = GetFakeUserPhotoList().SingleOrDefault(x => x.UserPhotoId == userPhotoId);
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetUserPhoto(userPhotoId)).ReturnsAsync(photoFromRepo);
            _repoMock.Setup(x => x.Delete(photoFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(true);

            // Act
            var result = _photosController.DeletePhoto(userId, userPhotoId).Result as OkResult;

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(new OkObjectResult(okResult).StatusCode, result.StatusCode);

        }
        
        [Fact]
        public void FindPhoto_PublicId_NotNull_DeletePhoto_Successful_ReturnsOk()
        {
            // Arrange
            int userId = 2;
            int userPhotoId = 3;
            var photoFromRepo = GetFakeUserPhotoList().SingleOrDefault(x => x.UserPhotoId == userPhotoId);
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);
            
            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetUserPhoto(userPhotoId)).ReturnsAsync(photoFromRepo);
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
            var result = _photosController.DeletePhoto(userId, userPhotoId).Result as OkResult;

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(new OkObjectResult(okResult).StatusCode, result.StatusCode);

        }
        
        [Fact]
        public void FindPhoto_DeletionFailsOnSave_ReturnsBadRequest()
        {
            // Arrange
            int userId = 2;
            int userPhotoId = 2;
            var photoFromRepo = GetFakeUserPhotoList().SingleOrDefault(x => x.UserPhotoId == userPhotoId);
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetUserPhoto(userPhotoId)).ReturnsAsync(photoFromRepo);
            _repoMock.Setup(x => x.Delete(photoFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            var result = _photosController.DeletePhoto(userId, userPhotoId).Result as BadRequestObjectResult;

            // Assert
            var okResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to delete the photo", okResult.Value);

        }
        
        [Fact]
        public void FindPhoto_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            int userId = 1;
            int userPhotoId = 2;
            var photoFromRepo = GetFakeUserPhotoList().SingleOrDefault(x => x.UserPhotoId == userPhotoId);
            var userFromRepo = GetFakeUserList().SingleOrDefault(x => x.Id == userId);

            _repoMock.Setup(x => x.GetUser(userId)).ReturnsAsync(userFromRepo);
            _repoMock.Setup(x => x.GetUserPhoto(userPhotoId)).ReturnsAsync(photoFromRepo);
            _repoMock.Setup(x => x.Delete(photoFromRepo));
            _repoMock.Setup(x => x.SaveAll()).ReturnsAsync(false);

            // Act
            var result = _photosController.DeletePhoto(userId, userPhotoId).Result;

            // Assert
            var okResult = Assert.IsType<UnauthorizedResult>(result);

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
                    UserPhotos = GetFakeUserPhotoList()
                    
                }
            };
        }

        private ICollection<UserPhoto> GetFakeUserPhotoList()
        {

            return new List<UserPhoto>()
            {
                
                new UserPhoto()
                {
                    UserId = 2,
                    UserPhotoId = 1,
                    Url = "http://res.cloudinary.com/deszzup5p/image/upload/v1592234384/RecipeApp/user_photos/z0ki54dbqgqbobwc6qpb.jpg",
                    IsMain = true,
                    PublicId = "RecipeApp/user_photos/z0ki54dbqgqbobwc6qpb",
                    Description = "My first photo!"

                    
                    
                },
                new UserPhoto()
                {
                    UserId = 2,
                    UserPhotoId = 2,
                    Url = "http://res.cloudinary.com/deszzup5p/image/upload/v1592234485/RecipeApp/user_photos/z5y795px3ac8afjawanb.jpg",
                    IsMain = false,
                    PublicId = null
                    
                },
                new UserPhoto()
                {
                    UserId = 2,
                    UserPhotoId = 3,
                    Url = "https://icatcare.org/app/uploads/2018/07/Thinking-of-getting-a-cat.png",
                    IsMain = false,
                    PublicId = "123456"
                    
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