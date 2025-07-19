using Core.Concretes.DTOs.Book;

namespace UI.Web.Models.Book
{
    public class BookDetailViewModel
    {
        public BookDetailDTO Book { get; set; } = null!;
        public List<BookListDTO> RelatedBooks { get; set; } = new();
    }
}
