using System.ComponentModel.DataAnnotations;

namespace apiUsuarios.DTOs.Users
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string SecondLastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "RoleId must be greater than 0.")]
        public int RoleId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "BranchId must be greater than 0.")]
        public int BranchId { get; set; }
    }
}
