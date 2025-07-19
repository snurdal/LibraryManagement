using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Concretes.DTOs.Book
{
    public class BookCreateDTO
    {
        public string Title { get; set; } = null!;
        public int PublishYear { get; set; }
        public string ShortContent { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? CoverImagePath { get; set; }
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
    }

}
