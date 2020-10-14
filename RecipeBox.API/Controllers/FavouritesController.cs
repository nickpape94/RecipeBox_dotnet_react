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
using Microsoft.AspNetCore.Identity;
using RecipeBox.API.Helpers;
using RecipeBox.API.Dtos.PostDtos;

namespace RecipeBox.API.Controllers
{
    [Route("api/[controller]/userId/{userId}")]
    [ApiController]
    public class FavouritesController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly IRecipeRepository _recipeRepo;
        
        public FavouritesController(IRecipeRepository recipeRepo, IMapper mapper)
        {
            _mapper = mapper;
            _recipeRepo = recipeRepo;
            
        }

        // Get posts from favourites
        [AllowAnonymous]
        [HttpPost(Name = "GetFavourites")]
        public async Task<IActionResult> GetFavourites(int userId, [FromQuery]PageParams pageParams, PostForSearchDto postForSearchDto)
        {
            var favourites = await _recipeRepo.GetFavourites(userId, pageParams, postForSearchDto);

            foreach(var favourite in favourites) 
            {
                var authorAvatar = await _recipeRepo.GetMainPhotoForUser(favourite.UserId);
                if (authorAvatar != null) favourite.UserPhotoUrl = authorAvatar.Url;

            }

            var favouritesFromRepo = _mapper.Map<IEnumerable<PostsForListDto>>(favourites);

            Response.AddPagination(favourites.CurrentPage, favourites.PageSize, favourites.TotalCount, favourites.TotalPages);

            return Ok(favouritesFromRepo);
        }


        // Add post to favourites
        [HttpPost("postId/{postId}")]
        public async Task<IActionResult> AddToFavourites(int userId, int postId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get user
            var userFromRepo = await _recipeRepo.GetUser(userId);

            // Find post
            var postFromRepo = await _recipeRepo.GetPost(postId);
            if (postFromRepo == null) return NotFound();

            // Add post to users favourites, and save
            var postForFavourite = new Favourite
            {
                PostId = postFromRepo.PostId,
                FavouriterId = userFromRepo.Id,
                Username = userFromRepo.UserName
            };

            // Check if post has already been favourited
            if (userFromRepo.Favourites.Any(x => x.PostId == postId)) return BadRequest("Recipe has already been favourited");

            userFromRepo.Favourites.Add(postForFavourite);

            if (await _recipeRepo.SaveAll())
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

            var favouriteFromRepo = await _recipeRepo.GetFavourite(postId, userId);

            if ( favouriteFromRepo == null) return NotFound($"Recipe with id {postId} not found in favourites");
            if (favouriteFromRepo.FavouriterId != userId) return Unauthorized();

            _recipeRepo.Delete(favouriteFromRepo);

            if (await _recipeRepo.SaveAll())
            {
                return Ok("Favourite successfully deleted");
            }

            return BadRequest("Could not delete photo from favourites");
        }


    }
}