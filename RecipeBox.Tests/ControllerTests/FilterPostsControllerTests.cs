using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
    public class FilterPostsControllerTests
    {
        private Mock<IRecipeRepository> _repoMock;
        private FilterPostsController _filterPostsController;
        private CalculateAverageRatings _calculateAverageRatings;

        public FilterPostsControllerTests()
        {
            _repoMock = new Mock<IRecipeRepository>();

            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfiles()); });

            var mapper = mockMapper.CreateMapper();

            _filterPostsController = new FilterPostsController(_repoMock.Object, mapper);

            _calculateAverageRatings = new CalculateAverageRatings(_repoMock.Object);
        }

        [Fact]
        public void SortPosts_ByMostDiscussed()
        {
            // Arrange
            var postsFromRepo = GetFakePosts();
            var postsSorted = postsFromRepo.OrderByDescending( r => r.Comments.Count);
            
            // Act
            _repoMock.Setup(x => x.GetPosts()).ReturnsAsync(postsFromRepo);

            var result = _filterPostsController.Sort(new PostForSearchDto {
                OrderBy = "most discussed"
            }).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(postsSorted , okResult.Value);


        }
        
        [Fact]
        public void SortPosts_ByOldest()
        {
            // Arrange
            var postsFromRepo = GetFakePosts();
            var postsSorted = postsFromRepo.OrderBy( r => r.Created);
            
            // Act
            _repoMock.Setup(x => x.GetPosts()).ReturnsAsync(postsFromRepo);

            var result = _filterPostsController.Sort(new PostForSearchDto {
                OrderBy = "oldest"
            }).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(postsSorted , okResult.Value);

        }
        
        [Fact]
        public void SortPosts_ByNewest()
        {
            // Arrange
            var postsFromRepo = GetFakePosts();
            var postsSorted = postsFromRepo.OrderByDescending( r => r.Created);
            
            // Act
            _repoMock.Setup(x => x.GetPosts()).ReturnsAsync(postsFromRepo);

            var result = _filterPostsController.Sort(new PostForSearchDto {
                OrderBy = "newest"
            }).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(postsSorted , okResult.Value);

        }
        
        [Fact]
        public void SortPosts_ByHighestRated()
        {
            // Arrange
            var postsFromRepo = GetFakePosts();
            // var postsSorted = postsFromRepo.OrderByDescending( r => r.Created);
            
            // Act
            _repoMock.Setup(x => x.GetPosts()).ReturnsAsync(postsFromRepo);

            var result = _filterPostsController.Sort(new PostForSearchDto {
                OrderBy = "highest rated"
            }).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Assert.Equal(postsSorted , okResult.Value);

        }

        [Fact]
        public void SortPosts_ByUser_User_Has_No_Posts_Returns_NotFound()
        {
            // Arrange
            int userId = 2;
            var userFromRepo = GetFakeUsers().SingleOrDefault(x => x.UserId == userId);
            var postsFromRepo = GetFakePosts();

            // Act
            _repoMock.Setup(x => x.GetPosts()).ReturnsAsync(postsFromRepo);

            var result = _filterPostsController.SortByUser(userId).Result;

            // Assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User has not submitted any posts", okResult.Value);
        }
        
        [Fact]
        public void SortPosts_ByUser_Returns_Posts_Made_By_User()
        {
            // Arrange
            int userId = 1;
            var userFromRepo = GetFakeUsers().SingleOrDefault(x => x.UserId == userId);
            var postsFromRepo = GetFakePosts();
            var filteredPosts = postsFromRepo.Where(x => x.UserId == userId);

            // Act
            _repoMock.Setup(x => x.GetPosts()).ReturnsAsync(postsFromRepo);

            var result = _filterPostsController.SortByUser(userId).Result;

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(filteredPosts ,okResult.Value);
        }

        [Fact]
        public void SearchPosts_ByName_OrCuisine_NoPostsMatch_Returns_NotFound()
        {
            // Arrange
            var postsFromRepo = GetFakePosts();

            // Act
            _repoMock.Setup(x => x.GetPosts()).ReturnsAsync(postsFromRepo);

            var result = _filterPostsController.SearchPosts( new PostForSearchDto {
                SearchParams = "qpworekdkrpirfv"
            }).Result;
            
            // Assert
            var okResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No posts match the criteria", okResult.Value);

        }

        [Fact]
        public void SearchPosts_ByName_OrCuisine_Gets_MatchingPosts_ReturnsOk()
        {
            // Arrange
            var searchParam = "Italian";
            var postsFromRepo = GetFakePosts();
            var searchPostsByCuisine = postsFromRepo.Where(x => x.Cuisine == searchParam).OrderByDescending(x => x.Created);

            // Act
            _repoMock.Setup(x => x.GetPosts()).ReturnsAsync(postsFromRepo);

            var result = _filterPostsController.SearchPosts( new PostForSearchDto {
                SearchParams = searchParam
            }).Result;
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(searchPostsByCuisine, okResult.Value);

        }


        private ICollection<Post> GetFakePosts()
        {
            return new List<Post>()
            {
                new Post()
                {
                    PostId = 1,
                    NameOfDish = "ragu",
                    Cuisine = "Italian",
                    Created = new DateTime(2011, 6, 10),
                    Comments = Post1Comments(),
                    UserId = 1
                    
                },
                new Post()
                {
                    PostId = 2,
                    NameOfDish = "steak & ale pie",
                    Cuisine = "British",
                    Created = new DateTime(2009, 4, 3),
                    Comments = Post2Comments(),
                    UserId = 1
                },
                new Post()
                {
                    PostId = 3,
                    NameOfDish = "pizza",
                    Cuisine = "Italian",
                    Created = new DateTime(2014, 4, 9),
                    Comments = Post3Comments(),
                    UserId = 1
                }
            };
        }
        

        private ICollection<Comment> Post1Comments()
        {
            return new List<Comment>()
            {
                new Comment()
                {
                    CommentId = 1,
                    Text = "comment 1",
                    PostId = 1
                }
            };
        }
        
        private ICollection<Comment> Post2Comments()
        {
            return new List<Comment>()
            {
                new Comment()
                {
                    CommentId = 2,
                    Text = "comment 1",
                    PostId = 2
                },
                new Comment()
                {
                    CommentId = 3,
                    Text = "comment 2",
                    PostId = 2
                },
                new Comment()
                {
                    CommentId = 4,
                    Text = "comment 3",
                    PostId = 2
                },
            };
        }
        
        private ICollection<Comment> Post3Comments()
        {
            return new List<Comment>()
            {
                new Comment()
                {
                    CommentId = 5,
                    Text = "comment 1",
                    PostId = 3
                },
                new Comment()
                {
                    CommentId = 6,
                    Text = "comment 2",
                    PostId = 3
                },
                
            };
        }

        private ICollection<User> GetFakeUsers()
        {
            return new List<User>()
            {
                new User()
                {
                    UserId = 1,
                    Username = "nick",
                    Posts = GetFakePosts()
                },
                new User()
                {
                    UserId = 2,
                    Username = "jim"
                }
            };
        }
    }
}