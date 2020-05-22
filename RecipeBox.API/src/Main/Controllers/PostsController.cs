using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.API.src.Main.Data;
using RecipeBox.API.src.Main.Dtos;
using RecipeBox.API.src.Main.Models;

namespace RecipeBox.API.src.Main.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    // [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _repo;
        public PostsController(IRecipeRepository repo, IMapper mapper)
        {
        
            _mapper = mapper;
            _repo = repo;
            
        }

        // Get all posts
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _repo.GetPosts();

            return Ok(posts);
        }

        // Get post by id
        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetPost")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _repo.GetPost(id);

            var postsFromRepo = _mapper.Map<PostsForDetailedDto>(post);

            return Ok(postsFromRepo);
        }

        // Create a post
        [HttpPost("~/api/users/{userId}/posts")]
        public async Task<IActionResult> CreatePost(int userId, PostForCreationDto postForCreationDto)
        {
            // Validate id of logged in user == userId
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get user with this id from the repo       
            var userFromRepo = await _repo.GetUser(userId);

            // Get post data from the client
            var post = _mapper.Map<Post>(postForCreationDto);

            userFromRepo.Posts.Add(post);
            
            var postToReturn = _mapper.Map<PostsForDetailedDto>(post);

            if (await _repo.SaveAll())
            {
                return CreatedAtRoute("GetPost", new {userId = userId, id = post.Id}, postToReturn);
            }

            throw new Exception("Creating the post failed on save");
        }

        // Update a post
        [HttpPut("~/api/users/{userId}/posts/{postId}")]
        public async Task<IActionResult> UpdatePost(int userId, int postId, PostForUpdateDto postForUpdateDto)
        {
            // Validate id of logged in user == userId
            if ( userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get both the user and post from repo
            var userFromRepo = await _repo.GetUser(userId);
            var postFromRepo = await _repo.GetPost(postId);

            // Check post was made by the user
            if (postFromRepo.UserId != userId) return Unauthorized(); 

            // Map updated post into the repo
            var postToUpdate = _mapper.Map(postForUpdateDto, postFromRepo);
            var postToReturn = _mapper.Map<PostsForDetailedDto>(postToUpdate);

            if ( await _repo.SaveAll())
                return CreatedAtRoute("GetPost", new {userId = userId, id = postFromRepo.Id}, postToReturn);

            throw new Exception($"Updating post {postId} failed on save");
        }

        // Delete a post
        [HttpDelete("~/api/users/{userId}/posts/{postId}")]
        public async Task<IActionResult> DeletePost(int userId, int postId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get post and user from the repo
            var userFromRepo = await _repo.GetUser(userId);
            var postFromRepo = await _repo.GetPost(postId);

            if (postFromRepo.UserId != userId) return Unauthorized();

            _repo.Delete(postFromRepo);

            if ( await _repo.SaveAll())
                return StatusCode(201, "Successfully deleted post");

            throw new Exception($"Deleting post {postId} failed on save");
        }
        
    }
}