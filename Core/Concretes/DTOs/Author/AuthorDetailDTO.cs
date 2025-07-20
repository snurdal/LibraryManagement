using Core.Concretes.DTOs.Book;

namespace Core.Concretes.DTOs.Author
{
    public partial class AuthorDTO
    {
        public class AuthorDetailDTO
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public string FullName => $"{FirstName} {LastName}";
            public string? Biography { get; set; }
            public string? PhotoPath { get; set; }
            public DateTime CreateDate { get; set; }
            public List<BookListDTO> Books { get; set; } = new();
        }
    }
}
