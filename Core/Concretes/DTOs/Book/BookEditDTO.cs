namespace Core.Concretes.DTOs.Book
{
    public class BookEditDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int PublishYear { get; set; }
        public string? ShortContent { get; set; }
        public string? Description { get; set; } = null!;
        public string? CoverImagePath { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
    }

}
