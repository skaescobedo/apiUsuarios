namespace apiUsuarios.DTOs.Users
{
    public class UserResponseDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? SecondLastName { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public int BranchId { get; set; }

        public string BranchName { get; set; } = string.Empty;
    }
}