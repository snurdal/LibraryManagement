namespace UI.Web.Models.Book
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int PublishYear { get; set; }
        public string? ShortContent { get; set; }
        public string? Description { get; set; }
        public string? CoverImagePath { get; set; }
        public string CategoryName { get; set; } = null!;
        public string AuthorFullName { get; set; } = null!;
        public DateTime CreateDate { get; set; }
    }
}
