using Core.Concretes.DTOs.Book;
using Core.Concretes.DTOs.Category;
using static Core.Concretes.DTOs.Author.AuthorDTO;

namespace UI.Web.Models.Book
{
    public class BookEditViewModel
    {
        public BookEditDTO Book { get; set; } = new();
        public List<CategoryListDTO> Categories { get; set; } = new();
        public List<AuthorListDTO> Authors { get; set; } = new();
        public IFormFile? PhotoFile { get; set; }
    }
}
