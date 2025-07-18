using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Concretes.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<Book> Books { get; set; } = [];
    }
}
