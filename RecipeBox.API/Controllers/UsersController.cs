using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.API.Data;
using RecipeBox.API.Dtos;
using RecipeBox.API.Dtos.UserDtos;

namespace RecipeBox.API.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRecipeRepository _repo;
        private readonly IMapper _mapper;
        private readonly IAuthRepository _authRepo;
        public UsersController(IRecipeRepository repo, IAuthRepository authRepo, IMapper mapper)
        {
            _authRepo = authRepo;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateUserEmail(int id, UserEmailForUpdateDto userEmailForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            
            var userFromRepo = await _repo.GetUser(id);
            var userExists = await _authRepo.UserExists(userEmailForUpdateDto.Email);

            if (userFromRepo == null) return NotFound();

            if (userExists) return BadRequest("Email already in use");

            userFromRepo.Email = userEmailForUpdateDto.Email;

            if ( await _repo.SaveAll())
            {
                return Ok("Email address updated successfully");
            }

            return BadRequest("Updating email address failed on save");
        }
    }
}