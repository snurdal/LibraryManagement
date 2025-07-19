using Core.Concretes.DTOs.Category;
using static Core.Concretes.DTOs.Author.AuthorDTO;

namespace Core.Concretes.DTOs.Book
{
    public class BookDetailDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int PublishYear { get; set; }
        public string? Description { get; set; }
        public string? CoverImagePath { get; set; }
        public DateTime CreateDate { get; set; }
        public CategoryDetailDTO Category { get; set; } = null!;
        public AuthorDetailDTO Author { get; set; } = null!;
    }

}
