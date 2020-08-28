using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos.UserDtos;
using RecipeBox.API.Helpers;
using RecipeBox.API.Models;

namespace RecipeBox.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _recipeRepo;
        public UsersController(IRecipeRepository recipeRepo, IMapper mapper)
        {
            _recipeRepo = recipeRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]PageParams pageParams)
        {
            var users = await _recipeRepo.GetUsers(pageParams);

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(usersToReturn);
        }
        
        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _recipeRepo.GetUser(id);
            
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpGet("currentUser")]
        // [AllowAnonymous]
        public async Task<IActionResult> GetUser()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // fails here

            var user = await _recipeRepo.GetUser(currentUserId);

            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            // var result = _userManager.VerifyUserTokenAsync

            return Ok(userToReturn);
        }

        // [HttpPost("{id}")]
        // public async Task<IActionResult> UpdateUserEmail(int id, UserEmailForUpdateDto userEmailForUpdateDto)
        // {
        //     if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            
        //     var userFromRepo = await _repo.GetUser(id);
        //     var userExists = await _authRepo.UserExists(userEmailForUpdateDto.Email);

        //     if (userFromRepo == null) return NotFound();

        //     if (userExists) return BadRequest("Email already in use");

        //     userFromRepo.Email = userEmailForUpdateDto.Email;

        //     if ( await _repo.SaveAll())
        //     {
        //         return Ok("Email address updated successfully");
        //     }

        //     return BadRequest("Updating email address failed on save");
        // }
    }
}