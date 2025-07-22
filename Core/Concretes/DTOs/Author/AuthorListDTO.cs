using Core.Concretes.DTOs.Book;

namespace Core.Concretes.DTOs.Author
{
    public partial class AuthorDTO
    {
        public class AuthorListDTO
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string FullName { get; set; } = null!;
            public string? PhotoPath { get; set; }
            public int BookCount { get; set; }
            public DateTime CreateDate { get; set; }
            public List<BookListDTO> Books { get; set; } = new();
        }
    }
}
