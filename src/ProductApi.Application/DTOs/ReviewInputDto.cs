namespace ProductApi.Application.DTOs
{
    public class ReviewInputDto
    {
        public int Stars { get; set; }  // e.g. 1 to 5
        public string Description { get; set; } = string.Empty;
    }
}
