using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab10.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public int NoOfPages { get; set; }
        public String BookName { get; set; }
        public int? CategoryID { get; set; }
        public String Image { get; set; }

        public Category Category { get; set; }

        
    }
}
