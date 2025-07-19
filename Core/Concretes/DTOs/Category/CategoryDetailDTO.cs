using Core.Concretes.DTOs.Book;

namespace Core.Concretes.DTOs.Category
{
    public class CategoryDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public List<BookListDTO> Books { get; set; } = new();
    }
}
