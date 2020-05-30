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

namespace RecipeBox.Tests
{
    public class PostsControllerTests
    {
        private Mock<IRecipeRepository> _repo;
        private readonly PostsController _controller;
        public PostsControllerTests()
        {
            _repo = new Mock<IRecipeRepository>();

            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfiles()); });

            var mapper = mockMapper.CreateMapper();

            _controller = new PostsController(_repo.Object, mapper);
        }

        [Fact]
        public void GetPost_WhenCalled_ReturnsRightPost()
        {
            // Arrange
            var post = GetFakePostList().SingleOrDefault(x => x.PostId == 1);
            _repo.Setup(repo => repo.GetPost(1)).ReturnsAsync(post);
            
            // Act
            var result = _controller.GetPost(1).Result;
            
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PostsForDetailedDto>(okResult.Value);
            Assert.Equal(post.NameOfDish, returnValue.NameOfDish);
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
                    Cuisine = "Italian"
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
                    Cuisine = "Italian"
                }
            };
        }
    }
}