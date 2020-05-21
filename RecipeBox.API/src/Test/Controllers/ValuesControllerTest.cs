using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBox.API.src.Main.Controllers;
using RecipeBox.API.src.Main.Data;
using Xunit;

namespace RecipeBox.API.src.Test.Controllers
{
    public class ValuesControllerTest 
    {
        
        private readonly DataContext _context;

    
        public ValuesControllerTest(DataContext context)
        {
            _context = context;    
        }
         
        [Fact]
        public async Task GetValues_ShouldReturnsValues()
        {
            // Arrange
            var values = await _context.Values.ToListAsync();

            // Act 

            // Assert
            Assert.Single(values);
        }

        [Fact]
        public void Test1()
        {
            int a = 5;
            int b = 5;

            Assert.Equal(a, b);
        }
    }
}