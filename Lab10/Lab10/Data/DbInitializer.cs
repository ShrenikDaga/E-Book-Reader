using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab10.Models;

namespace Lab10.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Categories.Any())
            {
                return;
            }

            var category = new Category[] {
                new Category { CategoryName = "Horror", NoOfBooks = 0},
                new Category { CategoryName = "Drama", NoOfBooks = 0},
                new Category { CategoryName = "Romantic", NoOfBooks = 0},
                new Category { CategoryName = "Comedy", NoOfBooks = 0}
            };

            foreach (Category c in category)
            {
                context.Categories.Add(c);
            }

            context.SaveChanges();

            var book = new Book[] {
                new Book { BookName = "Conjuring", NoOfPages = 400, CategoryID = 1},
                new Book { BookName = "Insedious", NoOfPages = 200, CategoryID = 1},
                new Book { BookName = "An Evening in Paris", NoOfPages = 300, CategoryID = 2},
                new Book { BookName = "Harry Potter", NoOfPages = 800, CategoryID = 2},
                new Book { BookName = "The Alchemist", NoOfPages = 300, CategoryID = 3}
            };

            foreach (Book b in book)
            {
                context.Books.Add(b);
            }

            context.SaveChanges();

            var comment = new Comment[] {
                new Comment { CommentData = "Sample Comment 1",BookID = 3,Commenter="shdaga@syr.edu"},
                new Comment { CommentData = "Sample Comment 2",BookID = 3,Commenter="shashank@gmail.com"},
                new Comment { CommentData = "Sample Comment 3",BookID = 3,Commenter="shashank@gmail.com"},
                new Comment { CommentData = "Sample Comment 4",BookID = 3,Commenter="arjun@gmail.com"},
            };

            foreach (Comment c in comment)
            {
                context.Comments.Add(c);
            }

            context.SaveChanges();
        }
    }
}
