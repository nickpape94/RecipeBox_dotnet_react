using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeBox.API.Data;
using RecipeBox.API.Models;
using Xunit;

namespace RecipeBox.Tests.RepositoryTests
{
    public class RecipeRepositoryTests
    {
        // [Fact]
        // public void GetAllPosts()
        // {
        //     // Arrange
        //     var dbContextMock = new Mock<DataContext>();
        //     var dbSetMock = new Mock<DbSet<Post>>();

        //     dbSetMock.Setup(x => x.FindAsync(It.IsAny<int>())).ReturnsAsync(new Post());

        //     dbContextMock.Setup(x => x.Set<Post>()).Returns(dbSetMock.Object);

        //     // Act
        //     var recipeRepository = new RecipeRepository(dbContextMock.Object);
        //     var post = recipeRepository.GetPost(It.IsAny<int>()).Result;

        //     // Assert
        //     Assert.NotNull(post);
        //     Assert.IsAssignableFrom<Post>(post);
        // }
    }
}