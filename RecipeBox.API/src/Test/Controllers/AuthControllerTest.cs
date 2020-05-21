using System.Threading.Tasks;
using Moq;
using RecipeBox.API.src.Main.Data;
using Xunit;

namespace RecipeBox.API.src.Test.Controllers
{

    public class AuthControllerTest
    {
        [Fact]
        public void Register_User()
        {
            // Arrange
            var mockRepo = new Mock<IAuthRepository>();
                
            

            // Act

            // Assert
            
        }
    }
}