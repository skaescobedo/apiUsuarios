namespace apiUsuarios.DTOs.Branches
{
    public class BranchResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;
        public string ExteriorNumber { get; set; } = string.Empty;
        public string? InteriorNumber { get; set; }
        public string? Neighborhood { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}