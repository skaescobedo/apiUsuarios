using System.ComponentModel.DataAnnotations;

namespace apiUsuarios.DTOs.Branches
{
    public class CreateBranchDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ExteriorNumber { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? InteriorNumber { get; set; }

        [MaxLength(100)]
        public string? Neighborhood { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;
    }
}