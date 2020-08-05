using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;

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


        [HttpPost]
        public async Task<IActionResult> Sort(PostForSearchDto postForSearch)
        {
            // sortOrder = JsonSerializer

            var posts = await _recipeRepo.GetPosts();
            var calculateAverageRatings = new CalculateAverageRatings(_recipeRepo);

            if (postForSearch.OrderBy == "most discussed") 
                posts = posts.OrderByDescending(x => x.Comments.Count);

            if (postForSearch.OrderBy == "oldest") 
                posts = posts.OrderBy(x => x.Created); 
                
            if (postForSearch.OrderBy == "newest")
                posts = posts.OrderByDescending(x => x.Created);

            if (postForSearch.OrderBy == "highest rated")
            {
                foreach (var post in posts)
                {
                    post.AverageRating = calculateAverageRatings.GetAverageRating(post.PostId).Result;
                }
                posts = posts.OrderByDescending(x => x.AverageRating);
            }
                
            return Ok(posts);
        }
        
        // // Sort by most discussed
        // [HttpGet("mostDiscussed")]
        // public async Task<IActionResult> SortByMostDiscussed()
        // {
        //     var posts = await _recipeRepo.GetPosts();

        //     posts = posts.OrderByDescending(x => x.Comments.Count);

        //     return Ok(posts);    
        // }
        
        
        // // Sort by newest
        // [HttpGet("newest")]
        // public async Task<IActionResult> SortByNewest()
        // {
        //     var posts = await _recipeRepo.GetPosts();
            
        //     posts = posts.OrderByDescending(x => x.Created);

        //     return Ok(posts);
        // }
        
        // // Sort by oldest
        // [HttpGet("oldest")]
        // public async Task<IActionResult> SortByOldest()
        // {
        //     var posts = await _recipeRepo.GetPosts();
            
        //     posts = posts.OrderBy(x => x.Created);

        //     return Ok(posts);
        // }


        // Sort by the user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> SortByUser(int userId)
        {
            var posts = await _recipeRepo.GetPosts();

            var filteredPosts = posts.Where(x => x.UserId == userId);

            if (filteredPosts.Count() == 0) return NotFound("User has not submitted any posts");

            return Ok(filteredPosts);
        }

        // Search by cuisine, name
        [HttpPost("search")]
        public async Task<IActionResult> SearchPosts(PostForSearchDto postForSearch)
        {
            var searchQueryToLower = postForSearch.SearchParams.Trim().ToLower();

            var posts = await _recipeRepo.GetPosts();

            var filteredPosts = posts.Where(x => x.NameOfDish.ToLower().Contains(searchQueryToLower) || 
                                                 x.Cuisine.ToLower().Contains(searchQueryToLower));

            filteredPosts = filteredPosts.OrderByDescending(x => x.Created);

            if (filteredPosts.Count() == 0) return NotFound("No posts match the criteria");


            return Ok(filteredPosts);

            
        }
        // Sort by highest rated
        
    }
}