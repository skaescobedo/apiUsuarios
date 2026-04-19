using apiUsuarios.Data;
using apiUsuarios.DTOs.Branches;
using apiUsuarios.Models;
using apiUsuarios.Services.Common;
using apiUsuarios.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace apiUsuarios.Services
{
    public class BranchService : IBranchService
    {
        private readonly AppDbContext _context;

        public BranchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BranchResponseDto>> GetAllAsync()
        {
            return await _context.Branches
                .AsNoTracking()
                .OrderBy(b => b.Id)
                .Select(b => new BranchResponseDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Street = b.Street,
                    ExteriorNumber = b.ExteriorNumber,
                    InteriorNumber = b.InteriorNumber,
                    Neighborhood = b.Neighborhood,
                    City = b.City,
                    State = b.State,
                    PostalCode = b.PostalCode,
                    Country = b.Country
                })
                .ToListAsync();
        }

        public async Task<BranchResponseDto?> GetByIdAsync(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null) return null;

            return MapToDto(branch);
        }

        public async Task<ServiceResult<BranchResponseDto>> CreateAsync(CreateBranchDto dto)
        {
            var validationError = ValidateAddressFields(dto.Name, dto.Street, dto.ExteriorNumber, dto.City, dto.State, dto.PostalCode, dto.Country);
            if (validationError != null)
            {
                return ServiceResult<BranchResponseDto>.Failure(ServiceErrorCode.Validation, validationError);
            }

            var normalizedName = dto.Name.Trim();
            var normalizedNameLower = normalizedName.ToLower();

            var exists = await _context.Branches.AnyAsync(b => b.Name.ToLower() == normalizedNameLower);
            if (exists)
            {
                return ServiceResult<BranchResponseDto>.Failure(ServiceErrorCode.Duplicate, "Branch name already exists.");
            }

            var branch = new Branch
            {
                Name = normalizedName,
                Street = dto.Street.Trim(),
                ExteriorNumber = dto.ExteriorNumber.Trim(),
                InteriorNumber = NormalizeOptional(dto.InteriorNumber),
                Neighborhood = NormalizeOptional(dto.Neighborhood),
                City = dto.City.Trim(),
                State = dto.State.Trim(),
                PostalCode = dto.PostalCode.Trim(),
                Country = dto.Country.Trim()
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return ServiceResult<BranchResponseDto>.Success(MapToDto(branch));
        }

        public async Task<ServiceResult<BranchResponseDto>> UpdateAsync(int id, UpdateBranchDto dto)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return ServiceResult<BranchResponseDto>.Failure(ServiceErrorCode.NotFound, $"Branch with id {id} was not found.");
            }

            var validationError = ValidateAddressFields(dto.Name, dto.Street, dto.ExteriorNumber, dto.City, dto.State, dto.PostalCode, dto.Country);
            if (validationError != null)
            {
                return ServiceResult<BranchResponseDto>.Failure(ServiceErrorCode.Validation, validationError);
            }

            var normalizedName = dto.Name.Trim();
            var normalizedNameLower = normalizedName.ToLower();

            var nameInUse = await _context.Branches.AnyAsync(b => b.Name.ToLower() == normalizedNameLower && b.Id != id);
            if (nameInUse)
            {
                return ServiceResult<BranchResponseDto>.Failure(ServiceErrorCode.Duplicate, "Branch name already exists.");
            }

            branch.Name = normalizedName;
            branch.Street = dto.Street.Trim();
            branch.ExteriorNumber = dto.ExteriorNumber.Trim();
            branch.InteriorNumber = NormalizeOptional(dto.InteriorNumber);
            branch.Neighborhood = NormalizeOptional(dto.Neighborhood);
            branch.City = dto.City.Trim();
            branch.State = dto.State.Trim();
            branch.PostalCode = dto.PostalCode.Trim();
            branch.Country = dto.Country.Trim();

            await _context.SaveChangesAsync();

            return ServiceResult<BranchResponseDto>.Success(MapToDto(branch));
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
            {
                return ServiceResult.Failure(ServiceErrorCode.NotFound, $"Branch with id {id} was not found.");
            }

            _context.Branches.Remove(branch);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ServiceResult.Failure(ServiceErrorCode.Conflict, "Branch cannot be deleted because it is assigned to one or more users.");
            }

            return ServiceResult.Success();
        }

        private static BranchResponseDto MapToDto(Branch branch)
        {
            return new BranchResponseDto
            {
                Id = branch.Id,
                Name = branch.Name,
                Street = branch.Street,
                ExteriorNumber = branch.ExteriorNumber,
                InteriorNumber = branch.InteriorNumber,
                Neighborhood = branch.Neighborhood,
                City = branch.City,
                State = branch.State,
                PostalCode = branch.PostalCode,
                Country = branch.Country
            };
        }

        private static string? NormalizeOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value.Trim();
        }

        private static string? ValidateAddressFields(
            string name,
            string street,
            string exteriorNumber,
            string city,
            string state,
            string postalCode,
            string country)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Branch name is required.";
            }

            if (string.IsNullOrWhiteSpace(street))
            {
                return "Street is required.";
            }

            if (string.IsNullOrWhiteSpace(exteriorNumber))
            {
                return "Exterior number is required.";
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                return "City is required.";
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                return "State is required.";
            }

            if (string.IsNullOrWhiteSpace(postalCode))
            {
                return "Postal code is required.";
            }

            if (string.IsNullOrWhiteSpace(country))
            {
                return "Country is required.";
            }

            return null;
        }
    }
}
