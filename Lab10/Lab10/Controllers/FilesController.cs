using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Lab10.Data;
using Lab10.Models;

namespace Lab10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IHostingEnvironment hostingEnvironment_;
        private string webRootPath = null;
        private string filePath = null;
        private readonly ApplicationDbContext context;

        public FilesController(IHostingEnvironment hostingEnvironment, ApplicationDbContext dbContext)
        {
            hostingEnvironment_ = hostingEnvironment;
            webRootPath = hostingEnvironment_.WebRootPath;
            filePath = Path.Combine(webRootPath, "File Storage");
            context = dbContext;
        }

        //----< Send back the list of all the categories avaiable >------
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            List<string> categoryList = new List<string>();
            var category = context.Categories.ToList();
            foreach (var c in category)
            {
                categoryList.Add(c.CategoryID.ToString()+"-"+c.CategoryName.ToString());
            }
            return categoryList;

        }

        //----< Send back the list of all the books avaiable >------
        [HttpGet("{id}/{idd}")]
        public IEnumerable<string> SendBookList(int id, int idd)
        {
            List<string> bookList = new List<string>();
            var books = context.Books.ToList();
            foreach (var b in books)
            {
                bookList.Add(b.BookID.ToString() + "-" + b.BookName.ToString());
            }
            return bookList;
        }


        //----< download single file in wwwroot\FileStorage >------
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Download(int id)
        {
            List<string> files = null;
            string file = "";
            try
            {
                files = Directory.GetFiles(filePath).ToList<string>();
                if (0 <= id && id < files.Count)
                    file = Path.GetFileName(files[id]);
                else
                    return NotFound();
            }
            catch
            {
                return NotFound();
            }
            var memory = new MemoryStream();
            file = files[id];
            using (var stream = new FileStream(file, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(file), Path.GetFileName(file));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".cs", "application/C#" },
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
        //----< upload file >--------------------------------------
        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var request = HttpContext.Request;
            var bookfile = request.Form.Files[0];
            var imgfile = request.Form.Files[1];

            {
                if (bookfile.Length > 0)
                {
                    var path = Path.Combine(filePath, bookfile.FileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await bookfile.CopyToAsync(fileStream);
                    }
                    
                    path = Path.Combine(filePath, imgfile.FileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imgfile.CopyToAsync(fileStream);
                    }

                    if (request.Form["Type"] == "Replace")
                    {
                        var book = context.Books.Find(Int32.Parse(request.Form["BookID"]));
                        book.BookName = bookfile.FileName;
                        book.Image = imgfile.FileName;

                        context.Update(book);
                        context.SaveChanges();
                    }
                    else
                    {
                        var book = new Book { BookName = bookfile.FileName, NoOfPages = Int32.Parse(request.Form["Pages"]), CategoryID = Int32.Parse(request.Form["CategoryID"]), Image = imgfile.FileName };
                        context.Books.Add(book);
                        context.SaveChanges();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            return Ok();
        }

    }
}