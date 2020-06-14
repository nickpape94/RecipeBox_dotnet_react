using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;

namespace RecipeBox.API.Controllers
{
    [Authorize]
    [Route("api/posts/{postId}/photos")]
    [ApiController]
    public class PostPhotosController : ControllerBase
    {
        private readonly IRecipeRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;
        
        public PostPhotosController(IRecipeRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            _repo = repo;

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
            var postPhotoFromRepo = await _repo.GetPostPhoto(id);

            var photo = _mapper.Map<PostPhotosForReturnDto>(postPhotoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoToPost(int postId, [FromForm]PostPhotoForCreationDto postPhotoForCreationDto)
        {
            var postFromRepo = await _repo.GetPost(postId);

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

            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PostPhotosForReturnDto>(photo);
                return CreatedAtRoute("GetPostPhotos", new {postId = postId, id = photo.PostPhotoId}, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int postId, int id)
        {
            var postFromRepo = await _repo.GetPost(postId);

            if (postFromRepo.UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) 
                return Unauthorized();

            if (!postFromRepo.PostPhoto.Any(p => p.PostPhotoId == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPostPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete the main photo");

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }

            if (photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int postId, int id)
        {
            var postFromRepo = await _repo.GetPost(postId);

            if (postFromRepo.UserId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            if (!postFromRepo.PostPhoto.Any(p => p.PostPhotoId == id))
                return Unauthorized();

            var photoFromRepo = await _repo.GetPostPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _repo.GetMainPhotoForPost(postId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");

            // TODO
            // Right now, first uploaded picture isnt being set to main, need to work on this so 1 photo is main on creation!!!!
            // Line 87
        }
            

        
    }
}