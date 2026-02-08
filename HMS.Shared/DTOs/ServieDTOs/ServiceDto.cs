namespace HMS.Shared.DTOs.ServieDTOs
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
