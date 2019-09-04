using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Extensions;
using Lab10.Models;
using Lab10.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Lab10.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment_;
        private readonly ApplicationDbContext _context;
        private IConfiguration _configuration;
        private const string sessionId_ = "SessionId";
        private string webRootPath = null;
        private string filePath = null;
        

        public BookController(IHostingEnvironment hostingEnvironment, ApplicationDbContext context, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            hostingEnvironment_ = hostingEnvironment;
            webRootPath = hostingEnvironment_.WebRootPath;
            _context = context;
            _configuration = configuration;
            filePath = Path.Combine(webRootPath, "File Storage");
        }

        public IActionResult Index()
        {
            return View();
        }

        public void UpdateCategoryCount()
        {
            var categories = _context.Categories.ToList();
            foreach (var cat in categories)
            {
                var bookList = _context.Books.Where(s => s.CategoryID == cat.CategoryID);
                _context.Categories.Find(cat.CategoryID).NoOfBooks = bookList.Count();
                _context.Update(cat);
                _context.SaveChanges();
            }

            
        }
        [AllowAnonymous]
        public IActionResult Books(int id)
        {
            UpdateCategoryCount();
            try
            {
                ViewData["CurrentID"] = id;
                ViewData["CurrentCategory"] = _context.Categories.Find(id).CategoryName;
                var bookList = _context.Books.Include(b => b.Category).Where(m => m.Category.CategoryID == id);
                return View(bookList.Select(b => b));
            }
            catch(Exception)
            {
               return RedirectToAction("Error", "Home");
            }
            
            
        }

        [HttpGet]
        public IActionResult ShowBook(string fileName, int? id, int catId)
        {
            try
            {
                var entireFileName = Path.Combine(filePath, fileName);
                

                if (!(_context.Categories.Any(o => o.CategoryID == catId) && _context.Books.Any(p => p.BookName == fileName) && _context.Books.Any(q=>q.BookID == id)))
                {
                    return RedirectToAction("Error", "Home"); 
                }

                ViewData["CurrentID"] = catId;
                ViewData["FileName"] = fileName;
                ViewData["BookID"] = id;

                var bookID = _context.Books.Find(id);

                if (User.IsInRole("Admin"))
                {
                    var commentListAdmin = _context.Comments
                        .Where(s => s.BookID == id);
                    return View(commentListAdmin);
                }

                var commentList = _context.Comments
                    .Where(s => s.BookID == id)
                    .Where(k => k.Commenter == User.Identity.Name || k.Commenter == _configuration.GetSection("AppSettings")["UserEmail"]);
                return View(commentList);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
            

        }
        //htpatel@syr.edu


        [HttpGet]
        public IActionResult CreateBook(int id)
        {
            HttpContext.Session.SetInt32(sessionId_, id);

            Category category = _context.Categories.Find(id);
            if (category == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            Book newBook = new Book();
            return View(newBook);
        }

        [HttpPost]
        public IActionResult CreateBook(int? id, Book book)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            int? categoryID = HttpContext.Session.GetInt32(sessionId_);

            var category = _context.Categories.Find(categoryID);

            if (category != null)
            {
                if (category.Books == null)
                {
                    List<Book> books = new List<Book>();
                    category.Books = books;
                }
                category.Books.Add(book);
                try
                {
                    _context.SaveChanges();
                }
                catch
                { }
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult AddComment(string filename,int id,int catId)
        {
            var comment = new Comment();
            //comment.BookID = id;
            return View(comment);
        }

        [HttpPost]
        public IActionResult AddComment(string filename, int id,int? catId, Comment comment)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            comment.BookID = id;
            comment.Commenter = User.Identity.Name;
            _context.Comments.Add(comment);
            _context.SaveChanges();

            filename = _context.Books.Find(id).BookName;
            catId = _context.Books.Find(id).CategoryID;

            return RedirectToAction("ShowBook", "Book", new { filename = filename,id = comment.BookID, catId = catId });
        }

        [HttpGet]
        public IActionResult EditBook(int? id)
        {
            if (id == null)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }

            Book book = _context.Books.Find(id);
            if (book == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            return View(book);
        }

        [HttpPost]
        public IActionResult EditBook(int? id, Book book)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            var dbBook = _context.Books.Find(id);
            if (dbBook != null)
            {
                dbBook.BookName = book.BookName;
                dbBook.NoOfPages = book.NoOfPages;
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {
                // do nothing for now
            }

            return RedirectToAction("Books","Book",new { id = dbBook.CategoryID });
        }

        public IActionResult DeleteBook(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            try
            {
                
                var book = _context.Books.Find(id);
                if (book != null)
                {
                    var comments = _context.Comments.Where(s => s.BookID == book.BookID);
                    if (comments != null)
                    {
                        foreach (var comment in comments)
                        {
                            _context.Comments.Remove(comment);
                        }
                    }
                    _context.Remove(book);
                    _context.SaveChanges();
                    return RedirectToAction("Books", "Book", new { id = book.CategoryID });
                }
            }
            catch
            { }

            return RedirectToAction("Index", "Home");
        }
    }
}