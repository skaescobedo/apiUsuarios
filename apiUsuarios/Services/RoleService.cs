using apiUsuarios.Data;
using apiUsuarios.DTOs.Roles;
using apiUsuarios.Models;
using apiUsuarios.Services.Common;
using apiUsuarios.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace apiUsuarios.Services
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _context;

        public RoleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleResponseDto>> GetAllAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Id)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();
        }

        public async Task<RoleResponseDto?> GetByIdAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return null;

            return MapToDto(role);
        }

        public async Task<ServiceResult<RoleResponseDto>> CreateAsync(CreateRoleDto dto)
        {
            var normalizedName = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return ServiceResult<RoleResponseDto>.Failure(ServiceErrorCode.Validation, "Role name is required.");
            }

            var normalizedNameLower = normalizedName.ToLower();
            var exists = await _context.Roles.AnyAsync(r => r.Name.ToLower() == normalizedNameLower);
            if (exists)
            {
                return ServiceResult<RoleResponseDto>.Failure(ServiceErrorCode.Duplicate, "Role name already exists.");
            }

            var role = new Role
            {
                Name = normalizedName,
                Description = NormalizeOptional(dto.Description)
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return ServiceResult<RoleResponseDto>.Success(MapToDto(role));
        }

        public async Task<ServiceResult<RoleResponseDto>> UpdateAsync(int id, UpdateRoleDto dto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return ServiceResult<RoleResponseDto>.Failure(ServiceErrorCode.NotFound, $"Role with id {id} was not found.");
            }

            var normalizedName = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return ServiceResult<RoleResponseDto>.Failure(ServiceErrorCode.Validation, "Role name is required.");
            }

            var normalizedNameLower = normalizedName.ToLower();
            var nameInUse = await _context.Roles.AnyAsync(r => r.Name.ToLower() == normalizedNameLower && r.Id != id);
            if (nameInUse)
            {
                return ServiceResult<RoleResponseDto>.Failure(ServiceErrorCode.Duplicate, "Role name already exists.");
            }

            role.Name = normalizedName;
            role.Description = NormalizeOptional(dto.Description);

            await _context.SaveChangesAsync();

            return ServiceResult<RoleResponseDto>.Success(MapToDto(role));
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return ServiceResult.Failure(ServiceErrorCode.NotFound, $"Role with id {id} was not found.");
            }

            _context.Roles.Remove(role);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ServiceResult.Failure(ServiceErrorCode.Conflict, "Role cannot be deleted because it is assigned to one or more users.");
            }

            return ServiceResult.Success();
        }

        private static string? NormalizeOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }

        private static RoleResponseDto MapToDto(Role role)
        {
            return new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
        }
    }
}
