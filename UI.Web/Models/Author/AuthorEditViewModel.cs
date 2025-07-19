using static Core.Concretes.DTOs.Author.AuthorDTO;

namespace UI.Web.Models.Author
{
    public class AuthorEditViewModel
    {
        public AuthorEditDTO Author { get; set; } = new();
    }
}
