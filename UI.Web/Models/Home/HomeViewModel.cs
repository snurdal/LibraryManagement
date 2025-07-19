using Core.Concretes.DTOs.Book;
using Core.Concretes.DTOs.Category;
using Core.Concretes.DTOs.Search;
using static Core.Concretes.DTOs.Author.AuthorDTO;

namespace UI.Web.Models.Home
{
    public class HomeViewModel
    {
        public List<BookListDTO> Books { get; set; } = new();
        public List<CategoryListDTO> Categories { get; set; } = new();
        public List<AuthorListDTO> Authors { get; set; } = new();
        public BookSearchDTO SearchCriteria { get; set; } = new();
        public int TotalBooks { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; } = 1;
    }
}
