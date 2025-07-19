using static Core.Concretes.DTOs.Author.AuthorDTO;

namespace UI.Web.Models.Author
{
    public class AuthorListViewModel
    {
        public List<AuthorListDTO> Authors { get; set; } = new();
    }
}
