using Core.Concretes.DTOs.Category;

namespace UI.Web.Models.Category
{
    public class CategoryListViewModel
    {
        public List<CategoryListDTO> Categories { get; set; } = new();
    }
}
