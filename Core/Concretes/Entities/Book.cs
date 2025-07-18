using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Concretes.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; } = null!;
        public int PublishYear { get; set; }
        public string? Description { get; set; }
        public string? CoverImagePath { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

    }
}
