using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Concretes.DTOs.Search
{
    public class BookSearchDTO
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? AuthorId { get; set; }
        public int? PublishYear { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
