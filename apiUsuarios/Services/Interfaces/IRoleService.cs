using apiUsuarios.DTOs.Roles;
using apiUsuarios.Services.Common;

namespace apiUsuarios.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponseDto>> GetAllAsync();
        Task<RoleResponseDto?> GetByIdAsync(int id);
        Task<ServiceResult<RoleResponseDto>> CreateAsync(CreateRoleDto dto);
        Task<ServiceResult<RoleResponseDto>> UpdateAsync(int id, UpdateRoleDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
