using Core.Concretes.DTOs.Book;
using Core.Concretes.DTOs.Category;
using static Core.Concretes.DTOs.Author.AuthorDTO;

namespace UI.Web.Models.Book
{
    public class BookCreateViewModel
    {
        public BookCreateDTO Book { get; set; } = new();
        public List<CategoryListDTO> Categories { get; set; } = new();
        public List<AuthorListDTO> Authors { get; set; } = new();

        // For creating new category/author on the fly
        public string? NewCategoryName { get; set; }
        public string? NewAuthorFirstName { get; set; }
        public string? NewAuthorLastName { get; set; }
        public IFormFile? PhotoFile { get; set; }
    }
}
