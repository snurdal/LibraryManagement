using System.ComponentModel.DataAnnotations;

namespace Core.Concretes.DTOs.Author
{
    public partial class AuthorDTO
    {
        public class AuthorEditDTO
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "First name is required")]
            [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
            public string FirstName { get; set; } = null!;

            [Required(ErrorMessage = "Last name is required")]
            [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
            public string LastName { get; set; } = null!;

            [StringLength(1000, ErrorMessage = "Biography cannot exceed 1000 characters")]
            public string? Biography { get; set; }

            public string? PhotoPath { get; set; }
        }
    }
}
