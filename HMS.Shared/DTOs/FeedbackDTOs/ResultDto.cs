namespace HMS.Shared.DTOs.FeedbackDTOs
{
    public class ResultDto
    {
        public bool Flagged { get; set; }
        public CategoriesDto Categories { get; set; } = null!;
    }
}
