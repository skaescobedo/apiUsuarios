using apiUsuarios.DTOs.Users;
using apiUsuarios.Services.Common;

namespace apiUsuarios.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync(int? roleId, int? branchId, string? search);
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserDto dto);
        Task<ServiceResult<UserResponseDto>> UpdateAsync(int id, UpdateUserDto dto);
        Task<ServiceResult> DeleteAsync(int id);
    }
}
