using System.ComponentModel.DataAnnotations;

namespace apiUsuarios.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Description { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}