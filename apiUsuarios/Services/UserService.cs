using apiUsuarios.Data;
using apiUsuarios.DTOs.Users;
using apiUsuarios.Models;
using apiUsuarios.Services.Common;
using apiUsuarios.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace apiUsuarios.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync(int? roleId, int? branchId, string? search)
        {
            var query = _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .AsQueryable();

            if (roleId.HasValue)
            {
                query = query.Where(u => u.RoleId == roleId.Value);
            }

            if (branchId.HasValue)
            {
                query = query.Where(u => u.BranchId == branchId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = $"%{search.Trim()}%";

                query = query.Where(u =>
                    EF.Functions.Like(u.FirstName, searchTerm) ||
                    EF.Functions.Like(u.LastName, searchTerm) ||
                    (u.SecondLastName != null && EF.Functions.Like(u.SecondLastName, searchTerm)) ||
                    EF.Functions.Like(u.Email, searchTerm) ||
                    EF.Functions.Like(u.Phone, searchTerm)
                );
            }

            return await query
                .OrderBy(u => u.Id)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SecondLastName = u.SecondLastName,
                    Email = u.Email,
                    Phone = u.Phone,
                    IsActive = u.IsActive,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    BranchId = u.BranchId,
                    BranchName = u.Branch.Name
                })
                .ToListAsync();
        }

        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Branch)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return null;
            }

            return MapToDto(user);
        }

        public async Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserDto dto)
        {
            if (HasInvalidRequiredFields(dto.FirstName, dto.LastName, dto.SecondLastName, dto.Email, dto.Phone))
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.Validation, "FirstName, LastName, Email and Phone are required.");
            }

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
            if (!roleExists)
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.Validation, $"Role with id {dto.RoleId} does not exist.");
            }

            var branchExists = await _context.Branches.AnyAsync(b => b.Id == dto.BranchId);
            if (!branchExists)
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.Validation, $"Branch with id {dto.BranchId} does not exist.");
            }

            var normalizedEmail = dto.Email.Trim().ToLower();
            var emailExists = await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail);
            if (emailExists)
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.Duplicate, "Email is already in use.");
            }

            var user = new User
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                SecondLastName = dto.SecondLastName.Trim(),
                Email = normalizedEmail,
                Phone = dto.Phone.Trim(),
                IsActive = dto.IsActive,
                RoleId = dto.RoleId,
                BranchId = dto.BranchId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var createdUser = await GetByIdAsync(user.Id);
            if (createdUser == null)
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.NotFound, $"User with id {user.Id} was not found.");
            }

            return ServiceResult<UserResponseDto>.Success(createdUser);
        }

        public async Task<ServiceResult<UserResponseDto>> UpdateAsync(int id, UpdateUserDto dto)
        {
            if (HasInvalidRequiredFields(dto.FirstName, dto.LastName, dto.SecondLastName, dto.Email, dto.Phone))
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.Validation, "FirstName, LastName, Email and Phone are required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.NotFound, $"User with id {id} was not found.");
            }

            var roleExists = await _context.Roles.AnyAsync(r => r.Id == dto.RoleId);
            if (!roleExists)
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.Validation, $"Role with id {dto.RoleId} does not exist.");
            }

            var branchExists = await _context.Branches.AnyAsync(b => b.Id == dto.BranchId);
            if (!branchExists)
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.Validation, $"Branch with id {dto.BranchId} does not exist.");
            }

            var normalizedEmail = dto.Email.Trim().ToLower();
            var emailInUse = await _context.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail && u.Id != id);
            if (emailInUse)
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.Duplicate, "Email is already in use.");
            }

            user.FirstName = dto.FirstName.Trim();
            user.LastName = dto.LastName.Trim();
            user.SecondLastName = dto.SecondLastName.Trim();
            user.Email = normalizedEmail;
            user.Phone = dto.Phone.Trim();
            user.IsActive = dto.IsActive;
            user.RoleId = dto.RoleId;
            user.BranchId = dto.BranchId;

            await _context.SaveChangesAsync();

            var updatedUser = await GetByIdAsync(user.Id);
            if (updatedUser == null)
            {
                return ServiceResult<UserResponseDto>.Failure(ServiceErrorCode.NotFound, $"User with id {user.Id} was not found.");
            }

            return ServiceResult<UserResponseDto>.Success(updatedUser);
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return ServiceResult.Failure(ServiceErrorCode.NotFound, $"User with id {id} was not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return ServiceResult.Success();
        }

        private static UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                SecondLastName = user.SecondLastName,
                Email = user.Email,
                Phone = user.Phone,
                IsActive = user.IsActive,
                RoleId = user.RoleId,
                RoleName = user.Role.Name,
                BranchId = user.BranchId,
                BranchName = user.Branch.Name
            };
        }

        private static bool HasInvalidRequiredFields(string firstName, string lastName, string secondLastName, string email, string phone)
        {
            return string.IsNullOrWhiteSpace(firstName)
                || string.IsNullOrWhiteSpace(lastName)
                || string.IsNullOrWhiteSpace(secondLastName)
                || string.IsNullOrWhiteSpace(email)
                || string.IsNullOrWhiteSpace(phone);
        }
    }
}
