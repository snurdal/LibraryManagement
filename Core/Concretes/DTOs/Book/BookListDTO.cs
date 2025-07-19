namespace Core.Concretes.DTOs.Book
{
    public class BookListDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int PublishYear { get; set; }
        public string? ShortDescription { get; set; }
        public string? CoverImagePath { get; set; }
        public string CategoryName { get; set; } = null!;
        public string AuthorFullName { get; set; } = null!;
        public DateTime CreateDate { get; set; }
    }

}
