using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeBox.API.Data;
using RecipeBox.API.Models;
using Xunit;
using Xunit.Sdk;

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

        // [Fact]
        // public void Add_TestClassObjectPassed_ProperMethodCalled()
        // {
        //     // Arrange
        //     var testObject = new TestClass();

        //     var context = new Mock<DataContext>();
        //     var dbSetMock = new Mock<DbSet<TestClass>>();
        //     context.Setup(x => x.Set<TestClass>()).Returns(dbSetMock.Object);
        //     dbSetMock.Setup(x => x.Add(It.IsAny<TestClass>())).Returns(testObject);

        //     // Act
        //     var repository = new Repository<TestClass>(context.Object);
        //     repository.Add(testObject);

        //     //Assert
        //     context.Verify(x => x.Set<TestClass>());
        //     dbSetMock.Verify(x => x.Add(It.Is<TestClass>(y => y == testObject)));
        // }

        [Fact]
        public void GetUser_Returns_User()
        {
            
        } 
    }
}