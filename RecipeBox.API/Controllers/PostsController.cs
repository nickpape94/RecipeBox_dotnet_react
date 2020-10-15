using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos.CommentDtos;
using RecipeBox.API.Dtos.PostDtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;

namespace RecipeBox.API.Controllers
{
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _recipeRepo; 
        public PostsController(IRecipeRepository recipeRepo, IMapper mapper)
        {
            _mapper = mapper;
            _recipeRepo = recipeRepo;
            
        }

        
        // Get all posts
        [AllowAnonymous]
        [HttpPost("~/api/posts")]
        public async Task<IActionResult> GetPosts([FromQuery]PageParams pageParams, PostForSearchDto postForSearchDto)
        {
            var posts = await _recipeRepo.GetPosts(pageParams, postForSearchDto);

            foreach (var post in posts)
            {
                // Assign users avatar to the post
                var authorAvatar = await _recipeRepo.GetMainPhotoForUser(post.UserId);
                if (authorAvatar != null) post.UserPhotoUrl = authorAvatar.Url;
                
            }

            var postsFromRepo = _mapper.Map<IEnumerable<PostsForListDto>>(posts);

            Response.AddPagination(posts.CurrentPage, posts.PageSize, posts.TotalCount, posts.TotalPages);

            return Ok(postsFromRepo);
        }
        
        // Get posts by user id
        // [AllowAnonymous]
        // [HttpPost("~/api/user/{userId}/posts")]
        // public async Task<IActionResult> GetPosts([FromQuery]PageParams pageParams, PostForSearchDto postForSearchDto, int userId)
        // {
        //     var posts = await _recipeRepo.GetPosts(userId, pageParams, postForSearchDto);

        //     foreach(var post in posts) 
        //     {
        //         var authorAvatar = await _recipeRepo.GetMainPhotoForUser(post.UserId);
        //         if (authorAvatar != null) post.UserPhotoUrl = authorAvatar.Url;
        //     }

        //     var postsFromRepo = _mapper.Map<IEnumerable<PostsForListDto>>(posts);

        //     Response.AddPagination(posts.CurrentPage, posts.PageSize, posts.TotalCount, posts.TotalPages);

        //     return Ok(postsFromRepo);
        // } 

        // Get post by id
        [AllowAnonymous]
        [HttpGet("~/api/posts/{id}", Name = "GetPost")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _recipeRepo.GetPost(id);

            // Assign users avatar to the post
            var authorAvatar = await _recipeRepo.GetMainPhotoForUser(post.UserId);
            if (authorAvatar != null) post.UserPhotoUrl = authorAvatar.Url;

            // Assign commenters photos
            foreach(var comment in post.Comments)
            {
                var commenterId = comment.CommenterId;
                var commentersMainPhoto = await _recipeRepo.GetMainPhotoForUser(commenterId);

                if (commentersMainPhoto != null )
                {
                    comment.UserPhotoUrl = commentersMainPhoto.Url;
                }
            }
            
            var postFromRepo = _mapper.Map<PostsForDetailedDto>(post);

            return Ok(postFromRepo);
        }

        // Create a post
        [HttpPost("~/api/users/{userId}/posts")]
        public async Task<IActionResult> CreatePost(int userId, PostForCreationDto postForCreationDto)
        {
            // Validate id of logged in user == userId
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFromRepo = await _recipeRepo.GetUser(userId);

            var post = _mapper.Map<Post>(postForCreationDto);

            post.Author = userFromRepo.UserName;

            userFromRepo.Posts.Add(post);
            
            if (await _recipeRepo.SaveAll())
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
            var postFromRepo = await _recipeRepo.GetPost(postId);

            // Check post was made by the user
            if (postFromRepo.UserId != userId) return Unauthorized(); 

            // Map updated post into the repo
            var postToUpdate = _mapper.Map(postForUpdateDto, postFromRepo);
            var postToReturn = _mapper.Map<PostsForDetailedDto>(postToUpdate);

            if ( await _recipeRepo.SaveAll())
                return CreatedAtRoute("GetPost", new {userId = userId, id = postFromRepo.PostId}, postToReturn);

            throw new Exception($"Updating post {postId} failed on save");
        }

        // Delete a post
        [HttpDelete("~/api/users/{userId}/posts/{postId}")]
        public async Task<IActionResult> DeletePost(int userId, int postId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get post from the repo
            var postFromRepo = await _recipeRepo.GetPost(postId);

            if (postFromRepo.UserId == userId)
            {
                _recipeRepo.Delete(postFromRepo);

                if (await _recipeRepo.SaveAll())
                    return Ok("Successfully deleted post");

                throw new Exception($"Deleting post {postId} failed on save");
            }

            return Unauthorized();
        }

        // Add comment to post
        [HttpPost("~/api/users/{userId}/posts/{postId}/comments")]
        public async Task<IActionResult> AddComment(int userId, int postId,  CommentForCreationDto commentForCreationDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFromRepo = await _recipeRepo.GetUser(userId);

            // Assign commenter id to the comment creator
            commentForCreationDto.CommenterId = userId;
            commentForCreationDto.Author = userFromRepo.UserName;

            // Get post from repo
            var postFromRepo = await _recipeRepo.GetPost(postId);

            if (postFromRepo == null) return NotFound($"Post with id {postId} not found");

            // Map comment into CommentForCreationDto
            var comment = _mapper.Map<Comment>(commentForCreationDto);
        
            // Add comment into comments
            postFromRepo.Comments.Add(comment);
            

            if (await _recipeRepo.SaveAll())
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

            var commentFromRepo = await _recipeRepo.GetComment(commentId);
            if (commentFromRepo == null) return NotFound($"Comment {commentId} not found");
            if (commentFromRepo.CommenterId != userId) return Unauthorized();

            var postFromRepo = await _recipeRepo.GetPost(commentFromRepo.PostId);
            
            var commentToUpdate = _mapper.Map(commentForUpdateDto, commentFromRepo);
            var commentToReturn = _mapper.Map<Comment>(commentToUpdate);
            // var commentToReturn = _mapper.Map<CommentsForReturnedDto>(commentToUpdate);

            if (await _recipeRepo.SaveAll())
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
            var comment = await _recipeRepo.GetComment(commentId);

            // Confirm user made the comment
            if (comment == null ) return NotFound($"Comment {commentId} not found");
            if (comment.CommenterId != userId) return Unauthorized();

            // Delete from repo
            _recipeRepo.Delete(comment);

            if (await _recipeRepo.SaveAll())
            {
                return Ok("Comment was successfully deleted");  
            }

            throw new Exception("Deleting the comment failed on save");
        }

        // Add a rating to a post
        [HttpPost("~/api/users/{userId}/posts/{postId}/ratings")]
        public async Task<IActionResult> AddRatingToPost(int userId, int postId, RatePostDto ratePostDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var postFromRepo = await _recipeRepo.GetPost(postId);
            var calculateAverageRatings = new CalculateAverageRatings(_recipeRepo);

            ratePostDto.RaterId = userId;

            if (postFromRepo.UserId == userId) return Unauthorized("You cannot rate your own recipe");

            var rating = _mapper.Map<Rating>(ratePostDto);

            // Check if user has already rated the post, and to replace if they have
            if ( postFromRepo.Ratings.Any(x => x.RaterId == userId))
            {
                var originalRating = await _recipeRepo.GetRating(userId, postId);
                originalRating.Score = ratePostDto.Score;

            }

            // If user has not yet rated this post, add new rating
            if (!postFromRepo.Ratings.Any(x => x.RaterId == userId))
            {
                postFromRepo.Ratings.Add(rating);
            }

            var average = calculateAverageRatings.GetAverageRating(postId).Result;
                
            postFromRepo.AverageRating = average;
            
            if (await _recipeRepo.SaveAll())
            {
                

                var postToReturn = _mapper.Map<PostsForDetailedDto>(postFromRepo);

                // postToReturn.AverageRating = average;
                return CreatedAtRoute("GetPost", new {userId = userId, id = rating.RatingId}, postToReturn);

            }

            return BadRequest("Failed to add a rating to post");

        }

        // Get average rating
        // [AllowAnonymous]
        // [HttpGet("~/api/posts/{postId}/ratings")]
        // public async Task<double> GetAverageRating(int postId)
        // {
        //     var ratingsForPost = await _repo.GetRatings(postId);

        //     double sum = 0;
        //     double numberOfRatings = ratingsForPost.Count();
            
        //     foreach (var rating in ratingsForPost)
        //     {
        //         sum += rating.Score;
        //     }

        //     if (numberOfRatings == 0) return 0.0;
            
        //     var averageRating = sum / numberOfRatings;

        //     return Math.Round(averageRating, 2);
        // }
    
    }
}