namespace Core.Concretes.DTOs.Category
{
    public class CategoryListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int BookCount { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
