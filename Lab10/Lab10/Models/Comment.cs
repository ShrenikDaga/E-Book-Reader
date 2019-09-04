using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab10.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        public String CommentData { get; set; }
        public int BookID { get; set; }
        public String Commenter { get; set; }

        public Book Book { get; set; }
    }
}
