using apiUsuarios.Controllers.Common;
using apiUsuarios.DTOs.Common;
using apiUsuarios.DTOs.Users;
using apiUsuarios.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace apiUsuarios.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IApiErrorMapper _errorMapper;

        public UsersController(IUserService userService, IApiErrorMapper errorMapper)
        {
            _userService = userService;
            _errorMapper = errorMapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll(
            [FromQuery] int? roleId,
            [FromQuery] int? branchId,
            [FromQuery] string? search)
        {
            var users = await _userService.GetAllAsync(roleId, branchId, search);
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound(new ApiErrorResponse { Message = $"User with id {id} was not found.", Code = "NotFound" });
            }

            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserDto dto)
        {
            var createdUser = await _userService.CreateAsync(dto);
            if (!createdUser.IsSuccess)
            {
                return _errorMapper.ToActionResult(this, createdUser.Error!);
            }

            return CreatedAtAction(nameof(GetById), new { id = createdUser.Value!.Id }, createdUser.Value);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponseDto>> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var updatedUser = await _userService.UpdateAsync(id, dto);
            if (!updatedUser.IsSuccess)
            {
                return _errorMapper.ToActionResult(this, updatedUser.Error!);
            }

            return Ok(updatedUser.Value);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return _errorMapper.ToActionResult(this, result.Error!);
            }

            return NoContent();
        }
    }
}
