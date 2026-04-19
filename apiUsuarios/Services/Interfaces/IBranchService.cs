using apiUsuarios.DTOs.Branches;
using apiUsuarios.Services.Common;

namespace apiUsuarios.Services.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchResponseDto>> GetAllAsync();
        Task<BranchResponseDto?> GetByIdAsync(int id);
        Task<ServiceResult<BranchResponseDto>> CreateAsync(CreateBranchDto dto);
        Task<ServiceResult<BranchResponseDto>> UpdateAsync(int id, UpdateBranchDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
