using Core.Concretes.DTOs.Category;

namespace UI.Web.Models.Category
{
    public class CategoryCreateViewModel
    {
        public CategoryCreateDTO Category { get; set; } = new();
    }
}
