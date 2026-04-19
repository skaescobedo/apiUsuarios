using System.ComponentModel.DataAnnotations;

namespace apiUsuarios.DTOs.Roles
{
    public class CreateRoleDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }
    }
}