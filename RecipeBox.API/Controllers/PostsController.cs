using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Models;

namespace RecipeBox.API.Controllers
{
    [Authorize]
    // [Route("api/[controller]")]
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
        [HttpGet("~/api/posts")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _repo.GetPosts();

            var postsFromRepo = _mapper.Map<IEnumerable<PostsForListDto>>(posts);

            return Ok(postsFromRepo);
        }

        // Get post by id
        [AllowAnonymous]
        [HttpGet("~/api/posts/{id}", Name = "GetPost")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _repo.GetPost(id);

            var postFromRepo = _mapper.Map<PostsForDetailedDto>(post);

            return Ok(postFromRepo);
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
            
            if (await _repo.SaveAll())
            {
                var postToReturn = _mapper.Map<PostsForDetailedDto>(post);
                return CreatedAtRoute("GetPost", new {userId = userId, id = post.PostId}, postToReturn);
            }

            throw new Exception("Creating the post failed on save");
        }

        // Update a post
        [HttpPut("~/api/users/{userId}/posts/{postId}")]
        public async Task<IActionResult> UpdatePost(int userId, int postId, PostForUpdateDto postForUpdateDto)
        {
            // Validate id of logged in user == userId
            if ( userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get post from repo
            var postFromRepo = await _repo.GetPost(postId);

            // Check post was made by the user
            if (postFromRepo.UserId != userId) return Unauthorized(); 

            // Map updated post into the repo
            var postToUpdate = _mapper.Map(postForUpdateDto, postFromRepo);
            var postToReturn = _mapper.Map<PostsForDetailedDto>(postToUpdate);

            if ( await _repo.SaveAll())
                return CreatedAtRoute("GetPost", new {userId = userId, id = postFromRepo.PostId}, postToReturn);

            throw new Exception($"Updating post {postId} failed on save");
        }

        // Delete a post
        [HttpDelete("~/api/users/{userId}/posts/{postId}")]
        public async Task<IActionResult> DeletePost(int userId, int postId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get post from the repo
            var postFromRepo = await _repo.GetPost(postId);

            if (postFromRepo.UserId == userId)
            {
                _repo.Delete(postFromRepo);

                if (await _repo.SaveAll())
                    return Ok("Successfully deleted post");

                throw new Exception($"Deleting post {postId} failed on save");
            }

            return Unauthorized();
        }

        // Add comment to post
        [HttpPost("~/api/users/{userId}/posts/{postId}/comments")]
        public async Task<IActionResult> AddComment(int userId, int postId,  CommentForCreationDto commentForCreationDto)
        {
            // Validate id of logged in user == userId
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Assign commenter id to the comment creator
            commentForCreationDto.CommenterId = userId;

            // Get post from repo
            var postFromRepo = await _repo.GetPost(postId);

            if (postFromRepo == null) return NotFound($"Post with id {postId} not found");

            // Map comment into CommentForCreationDto
            var comment = _mapper.Map<Comment>(commentForCreationDto);
        
            // Add comment into comments
            postFromRepo.Comments.Add(comment);
            

            if (await _repo.SaveAll())
            {
                var postToReturn = _mapper.Map<PostsForDetailedDto>(postFromRepo);
                
                return CreatedAtRoute("GetPost", new {userId = userId, id = comment.CommentId}, postToReturn);
            }

            throw new Exception("Creating the comment failed on save");
        }
        
        // Update comment
        [HttpPut("~/api/users/{userId}/comments/{commentId}")]
        public async Task<IActionResult> UpdateComment(int userId, int commentId, CommentForUpdateDto commentForUpdateDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var commentFromRepo = await _repo.GetComment(commentId);
            if (commentFromRepo == null) return NotFound($"Comment {commentId} not found");
            if (commentFromRepo.CommenterId != userId) return Unauthorized();

            var postFromRepo = await _repo.GetPost(commentFromRepo.PostId);
            
            var commentToUpdate = _mapper.Map(commentForUpdateDto, commentFromRepo);
            var commentToReturn = _mapper.Map<Comment>(commentToUpdate);
            // var commentToReturn = _mapper.Map<CommentsForReturnedDto>(commentToUpdate);

            if (await _repo.SaveAll())
            {
                var postToReturn = _mapper.Map<PostsForDetailedDto>(postFromRepo);

                return CreatedAtRoute("GetPost", new {userId = userId, id = postFromRepo.PostId} , postToReturn);

            }
                
            throw new Exception("Updating the comment failed on save");
        }
        
        // Delete comment from post
        [HttpDelete("~/api/users/{userId}/comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId, int userId)
        {
            // Validate id of logged in user == userId
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get comment from repo
            var comment = await _repo.GetComment(commentId);

            // Confirm user made the comment
            if (comment == null ) return NotFound($"Comment {commentId} not found");
            if (comment.CommenterId != userId) return Unauthorized();

            // Delete from repo
            _repo.Delete(comment);

            if (await _repo.SaveAll())
            {
                return Ok("Comment was successfully deleted");  
            }

            throw new Exception("Deleting the comment failed on save");
        }
    
    }
}