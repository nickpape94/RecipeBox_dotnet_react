using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.API.Data;
using RecipeBox.API.Models;
using System;
using System.Collections.Generic;
using RecipeBox.API.Dtos;

namespace RecipeBox.API.Controllers
{
    [Authorize]
    [Route("api/[controller]/userId/{userId}")]
    [ApiController]
    public class FavouritesController : ControllerBase
    {
        private readonly IRecipeRepository _repo;
        public FavouritesController(IRecipeRepository repo)
        {
            _repo = repo;
            
        }

        // Get posts from favourites
        [AllowAnonymous]
        [HttpGet(Name = "GetFavourites")]
        public async Task<IActionResult> GetFavourites(int userId)
        {
            var favouritesFromRepo = await _repo.GetFavourites(userId);

            var postsFromRepo = await _repo.GetPosts();
            
            var filteredPosts = postsFromRepo.Where(x => favouritesFromRepo.Any(y => y.PostId == x.PostId));
            // var postsFromRepo = _mapper.Map<IEnumerable<FavouritesForListDto>>(favouritesFromRepo);

            return Ok(filteredPosts);
        }


        // Add post to favourites
        [HttpPost("postId/{postId}")]
        public async Task<IActionResult> AddToFavourites(int userId, int postId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get user
            var userFromRepo = await _repo.GetUser(userId);

            // Find post
            var postFromRepo = await _repo.GetPost(postId);
            if (postFromRepo == null) return NotFound();

            // Add post to users favourites, and save
            var postForFavourite = new Favourite
            {
                PostId = postFromRepo.PostId,
                FavouriterId = userFromRepo.UserId
            };

            // Check if post has already been favourited
            if (userFromRepo.Favourites.Any(x => x.PostId == postId)) return BadRequest("Recipe has already been favourited");

            userFromRepo.Favourites.Add(postForFavourite);

            if (await _repo.SaveAll())
            {
                return Ok("Recipe added successfully");
            }

            throw new Exception("Adding the post to favourites failed on save");
        }

        // Delete post from favourites
        [HttpDelete("postId/{postId}")]
        public async Task<IActionResult> DeleteAFavourite(int userId, int postId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var favouritesFromRepo = await _repo.GetFavourites(userId);

            var favouriteToDelete = favouritesFromRepo.FirstOrDefault(x => x.PostId == postId);

            if ( favouriteToDelete == null) return NotFound($"Recipe with id {postId} not found in favourites");
            if (favouriteToDelete.FavouriterId != userId) return Unauthorized();

            _repo.Delete(favouriteToDelete);

            if (await _repo.SaveAll())
            {
                return Ok("Favourite successfully deleted");
            }

            return BadRequest("Could not delete photo from favourites");
        }


    }
}