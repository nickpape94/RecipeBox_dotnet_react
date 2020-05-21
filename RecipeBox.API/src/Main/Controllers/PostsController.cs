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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _repo;
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _userRepo;
        public PostsController(IPostRepository repo, IRecipeRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _repo = repo;
            
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _repo.GetPosts();

            var postsFromRepo = _mapper.Map<IEnumerable<PostsForListDto>>(posts);

            return Ok(postsFromRepo);
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetPost")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _repo.GetPost(id);

            var postsFromRepo = _mapper.Map<PostsForDetailedDto>(post);

            return Ok(postsFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(int userId, [FromBody]PostForCreationDto postForCreationDto)
        {
            // Validate id of logged in user == userId
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get user with this id from the repo       
            var userFromRepo = await _userRepo.GetUser(userId);

            // Get post data from the client
            var post = _mapper.Map<Post>(postForCreationDto);
            
            userFromRepo.Posts.Add(post);

            var postToReturn = _mapper.Map<PostForCreationDto>(post);

            if (await _repo.SaveAll())
            {
                return CreatedAtRoute("GetPost", new {userId = userId, id = post.Id}, postToReturn);
            }

            throw new Exception("Creating the post failed on save");
        }
        
    }
}