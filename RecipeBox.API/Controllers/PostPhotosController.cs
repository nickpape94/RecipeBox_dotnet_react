using RecipeBox.API.Data;

namespace RecipeBox.API.Controllers
{
    public class PostPhotosController
    {
        private readonly IRecipeRepository _repo;
        public PostPhotosController(IRecipeRepository repo)
        {
            _repo = repo;
            
        }
    }
}