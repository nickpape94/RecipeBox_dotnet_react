using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Models;

namespace RecipeBox.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/sort")]
    public class FilterPostsController : ControllerBase
    {
        private readonly IRecipeRepository _repo;
        private readonly IMapper _mapper;

        public FilterPostsController(IRecipeRepository repo, IMapper mapper )
        {
            _mapper = mapper;
            _repo = repo;
            
        }
        
        // Sort by most discussed
        [HttpGet("mostDiscussed")]
        public async Task<IActionResult> SortByMostDiscussed()
        {
            var posts = await _repo.GetPosts();

            posts = posts.OrderByDescending(x => x.Comments.Count);

            return Ok(posts);    
        }
        
        
        // Sort by newest
        [HttpGet("newest")]
        public async Task<IActionResult> SortByNewest()
        {
            var posts = await _repo.GetPosts();
            
            posts = posts.OrderByDescending(x => x.Created);

            return Ok(posts);
        }
        
        // Sort by oldest
        [HttpGet("oldest")]
        public async Task<IActionResult> SortByOldest()
        {
            var posts = await _repo.GetPosts();
            
            posts = posts.OrderBy(x => x.Created);

            return Ok(posts);
        }


        // Sort by the user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> SortByUser(int userId)
        {
            var posts = await _repo.GetPosts();

            var filteredPosts = posts.Where(x => x.UserId == userId);

            if (filteredPosts.Count() == 0) return NotFound("User has not submitted any posts");

            return Ok(filteredPosts);
        }

        // Search by cuisine
        [HttpPost("search")]
        public async Task<IActionResult> SearchPosts(PostForSearch postForSearch)
        {
            var toLowerCase = postForSearch.SearchParams.Trim().ToLower();
            // var splitWords = toLowerCase.Split(" ");
            // IEnumerable<Post> filteredPosts = new IEnumerable<Post>();

            var posts = await _repo.GetPosts();

            var filteredPosts = posts.Where(x => x.NameOfDish.ToLower().Contains(toLowerCase) || 
                                                 x.Cuisine.ToLower().Contains(toLowerCase));

            if (filteredPosts.Count() == 0) return NotFound("No posts match the criteria");

            // foreach (var word in splitWords)
            // {
            //     var filteredPosts = posts.Where(x => x.NameOfDish.ToLower().Contains(word));
            //     if (filteredPosts == null) return NotFound("No recipes match this criteria");

            // }

            return Ok(filteredPosts);

            
        }
        // Sort by highest rated
        
    }
}