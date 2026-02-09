namespace HMS.Shared.DTOs.ServieDTOs
{
    public class ServiceRequestDto
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? StaffName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
