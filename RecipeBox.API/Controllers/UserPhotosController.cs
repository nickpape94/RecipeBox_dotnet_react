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
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class UserPhotosController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepo;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IMapper _mapper;
        private Cloudinary _cloudinary;

        public UserPhotosController(IRecipeRepository recipeRepo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
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

        // Get photo
        [HttpPost("{id}", Name = "GetUserPhoto")]
        public async Task<IActionResult> GetUserPhoto(int id)
        {
            var userPhotoFromRepo = await _recipeRepo.GetUserPhoto(id);

            var photo = _mapper.Map<UserPhotosForReturnDto>(userPhotoFromRepo);

            return Ok(photo);
        }

        // Add a photo   
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]UserPhotoForCreationDto userPhotoForCreationDto)
        {
            
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var userFromRepo = await _recipeRepo.GetUser(userId);

            // Delete existing photo if exists
            var currentUserPhoto = userFromRepo.UserPhotos.FirstOrDefault();

            if (currentUserPhoto != null)
            {
                if (currentUserPhoto.PublicId != null)
                {
                    var deleteParams = new DeletionParams(currentUserPhoto.PublicId);

                    var result = _cloudinary.Destroy(deleteParams);

                    if (result.Result == "ok")
                    {
                        _recipeRepo.Delete(currentUserPhoto);
                    }
                }

                if (currentUserPhoto.PublicId == null)
                {
                    _recipeRepo.Delete(currentUserPhoto);
                }
            }

            // Adding new photo
            var file = userPhotoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Folder = "RecipeApp/user_photos/",
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")

                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            userPhotoForCreationDto.Url = uploadResult.Url.ToString();
            userPhotoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<UserPhoto>(userPhotoForCreationDto);

            userFromRepo.UserPhotos.Add(photo);

            if (await _recipeRepo.SaveAll())
            {
                var photoToReturn = _mapper.Map<UserPhotosForReturnDto>(photo);
                return CreatedAtRoute("GetUserPhoto", new { userId = userId, id = photo.UserPhotoId}, photoToReturn);

            }

            return BadRequest("Could not add the photo");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _recipeRepo.GetUser(userId);

            if (!userFromRepo.UserPhotos.Any(p => p.UserPhotoId == id))
                return Unauthorized();

            var photoFromRepo = await _recipeRepo.GetUserPhoto(id);

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

    }
}