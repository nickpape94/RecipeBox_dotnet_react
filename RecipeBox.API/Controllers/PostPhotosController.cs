using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos.PhotoDtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;

namespace RecipeBox.API.Controllers
{
    [Route("api/posts/{postId}/photos")]
    [ApiController]
    public class PostPhotosController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        
        public PostPhotosController(IRecipeRepository recipeRepo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            _recipeRepo = recipeRepo;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);

        }

        [HttpGet("{id}", Name ="GetPostPhotos")]
        public async Task<IActionResult> GetPostPhoto(int id)
        {
            var postPhotoFromRepo = await _recipeRepo.GetPostPhoto(id);

            var photo = _mapper.Map<PostPhotosForReturnDto>(postPhotoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoToPost(int postId, [FromForm]PostPhotoForCreationDto postPhotoForCreationDto)
        {
            var postFromRepo = await _recipeRepo.GetPost(postId);

            if (postFromRepo == null) return NotFound();

            if (postFromRepo.UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var file = postPhotoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Folder = "RecipeApp/post_photos/",
                        Transformation = new Transformation().Width(1000).Height(1000).Crop("fill")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            postPhotoForCreationDto.Url = uploadResult.Url.ToString();
            postPhotoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<PostPhoto>(postPhotoForCreationDto);

            if (!postFromRepo.PostPhoto.Any(u => u.IsMain))
                photo.IsMain = true;

            postFromRepo.PostPhoto.Add(photo);

            if (await _recipeRepo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PostPhotosForReturnDto>(photo);
                return CreatedAtRoute("GetPostPhotos", new {postId = postId, id = photo.PostPhotoId}, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int postId, int id)
        {
            var postFromRepo = await _recipeRepo.GetPost(postId);

            if (postFromRepo.UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();

            if (!postFromRepo.PostPhoto.Any(p => p.PostPhotoId == id))
                return Unauthorized();

            var photoFromRepo = await _recipeRepo.GetPostPhoto(id);

            // if (photoFromRepo.IsMain)
            //     return BadRequest("You cannot delete the main photo");

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    _recipeRepo.Delete(photoFromRepo);
                }
            }

            if (photoFromRepo.PublicId == null)
            {
                _recipeRepo.Delete(photoFromRepo);
            }

            if (await _recipeRepo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int postId, int id)
        {
            var postFromRepo = await _recipeRepo.GetPost(postId);

            if (postFromRepo.UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            if (!postFromRepo.PostPhoto.Any(p => p.PostPhotoId == id))
                return Unauthorized();

            var photoFromRepo = await _recipeRepo.GetPostPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _recipeRepo.GetMainPhotoForPost(postId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _recipeRepo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");

            // TODO
            // Right now, first uploaded picture isnt being set to main, need to work on this so 1 photo is main on creation!!!!
            // Line 87
        }

    
        // Delete a post and associated posts stored in cloudinary
        [HttpDelete("~/api/users/{userId}/posts/{postId}")]
        public async Task<IActionResult> DeletePost(int userId, int postId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            // Get post from the repo
            var postFromRepo = await _recipeRepo.GetPost(postId);

            if (postFromRepo.UserId == userId)
            {
                foreach(var photo in postFromRepo.PostPhoto)
                {
                    var photoFromRepo = await _recipeRepo.GetPostPhoto(photo.PostPhotoId);

                    if (photoFromRepo.PublicId != null)
                    {
                        var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                        var result = _cloudinary.Destroy(deleteParams);

                        if (result.Result == "ok")
                        {
                            _recipeRepo.Delete(photoFromRepo);
                        }
               
                    }
                }
                
                _recipeRepo.Delete(postFromRepo);

                if (await _recipeRepo.SaveAll())
                    return Ok("Successfully deleted post");

                throw new Exception($"Deleting post {postId} failed on save");
            }

            return Unauthorized();
        }
            

        
    }
}