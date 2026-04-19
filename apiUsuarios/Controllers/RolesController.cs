using apiUsuarios.Controllers.Common;
using apiUsuarios.DTOs.Common;
using apiUsuarios.DTOs.Roles;
using apiUsuarios.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace apiUsuarios.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IApiErrorMapper _errorMapper;

        public RolesController(IRoleService roleService, IApiErrorMapper errorMapper)
        {
            _roleService = roleService;
            _errorMapper = errorMapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleResponseDto>> GetById(int id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
                return NotFound(new ApiErrorResponse { Message = $"Role with id {id} was not found.", Code = "NotFound" });

            return Ok(role);
        }

        [HttpPost]
        [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleResponseDto>> Create([FromBody] CreateRoleDto dto)
        {
            var createdRole = await _roleService.CreateAsync(dto);
            if (!createdRole.IsSuccess)
            {
                return _errorMapper.ToActionResult(this, createdRole.Error!);
            }

            return CreatedAtAction(nameof(GetById), new { id = createdRole.Value!.Id }, createdRole.Value);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(RoleResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleResponseDto>> Update(int id, [FromBody] UpdateRoleDto dto)
        {
            var updatedRole = await _roleService.UpdateAsync(id, dto);
            if (!updatedRole.IsSuccess)
            {
                return _errorMapper.ToActionResult(this, updatedRole.Error!);
            }

            return Ok(updatedRole.Value);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roleService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return _errorMapper.ToActionResult(this, result.Error!);
            }

            return NoContent();
        }
    }
}
