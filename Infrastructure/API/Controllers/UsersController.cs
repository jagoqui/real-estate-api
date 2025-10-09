using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates a new user.")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto request)
        {
            try
            {
                var createdUser = await _service.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Retrieves all users. Admin role required.")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _service.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieves a user by their ID. Admin role required.")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            try
            {
                var user = await _service.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("without-owner")]
        [SwaggerOperation(Summary = "Retrieves users not associated with any owners. Admin role required.")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersWithoutOwners()
        {
            var users = await _service.GetUsersWithoutOwnersAsync();
            return Ok(users);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates an existing user by their ID.")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, [FromForm] UserWithFileDto user)
        {
            var updatedUser = await _service.UpdateAsync(id, user);
            return updatedUser == null ? NotFound() : Ok(updatedUser);
        }

        [HttpPost("{id}/change-password")]
        [SwaggerOperation(Summary = "Changes a user's password.")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> ChangePasswordAsync([FromRoute] string id, [FromBody] string newPassword)
        {
            var request = new ChangeUserPasswordRequest
            {
                UserId = id,
                NewPassword = newPassword,
            };

            try
            {
                var user = await _service.ChangePasswordAsync(request);
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes a user by their ID. Admin role required.")]
        [Authorize(Roles = nameof(UserRole.ADMIN))]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await _service.DeleteUserAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
