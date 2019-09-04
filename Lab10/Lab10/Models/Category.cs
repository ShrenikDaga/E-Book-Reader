using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab10.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public int NoOfBooks { get; set; }
        public String CategoryName { get; set; }
        public String Image { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
