using apiUsuarios.Controllers.Common;
using apiUsuarios.DTOs.Common;
using apiUsuarios.DTOs.Branches;
using apiUsuarios.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace apiUsuarios.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;
        private readonly IApiErrorMapper _errorMapper;

        public BranchesController(IBranchService branchService, IApiErrorMapper errorMapper)
        {
            _branchService = branchService;
            _errorMapper = errorMapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BranchResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BranchResponseDto>>> GetAll()
        {
            var branches = await _branchService.GetAllAsync();
            return Ok(branches);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BranchResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BranchResponseDto>> GetById(int id)
        {
            var branch = await _branchService.GetByIdAsync(id);

            if (branch == null)
                return NotFound(new ApiErrorResponse { Message = $"Branch with id {id} was not found.", Code = "NotFound" });

            return Ok(branch);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BranchResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BranchResponseDto>> Create([FromBody] CreateBranchDto dto)
        {
            var createdBranch = await _branchService.CreateAsync(dto);
            if (!createdBranch.IsSuccess)
            {
                return _errorMapper.ToActionResult(this, createdBranch.Error!);
            }

            return CreatedAtAction(nameof(GetById), new { id = createdBranch.Value!.Id }, createdBranch.Value);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(BranchResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BranchResponseDto>> Update(int id, [FromBody] UpdateBranchDto dto)
        {
            var updatedBranch = await _branchService.UpdateAsync(id, dto);
            if (!updatedBranch.IsSuccess)
            {
                return _errorMapper.ToActionResult(this, updatedBranch.Error!);
            }

            return Ok(updatedBranch.Value);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _branchService.DeleteAsync(id);
            if (!result.IsSuccess)
            {
                return _errorMapper.ToActionResult(this, result.Error!);
            }

            return NoContent();
        }
    }
}
