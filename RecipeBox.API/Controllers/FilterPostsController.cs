using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos.PostDtos;
using RecipeBox.API.Helpers;

namespace RecipeBox.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/sort")]
    public class FilterPostsController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IMapper _mapper;

        public FilterPostsController(IRecipeRepository recipeRepo, IMapper mapper)
        {
            _mapper = mapper;
            _recipeRepo = recipeRepo;
        }


        // Sort by the user
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> SortByUser(int userId)
        {
            var posts = await _recipeRepo.GetPosts();

            var filteredPosts = posts.Where(x => x.UserId == userId);

            if (filteredPosts.Count() == 0) return NotFound("User has not submitted any posts");

            return Ok(filteredPosts);
        }

        
    }
}