using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Extensions;
using Lab10.Models;
using Lab10.Data;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace Lab10.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private const string sessionId_ = "SessionId";

        public HomeController(ApplicationDbContext context,IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            //return View();
            return View(_context.Categories.ToList<Category>());
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateCategory()
        {
            var model = new Category();
            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> CreateCategory(Category category,IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("Image not Selected");
            string path_root = _hostingEnvironment.WebRootPath;
            string path_to_Images = path_root + "\\Pictures\\CategoryImages\\" + file.FileName;
            //Moving file to target
            using (var stream = new FileStream(path_to_Images, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            category.Image = file.FileName;
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        //-----------------------<Delete Category>---------------------
        public IActionResult DeleteCategory(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            try
            {
                var category = _context.Categories.Find(id);
                if (category != null)
                {
                    var bookList = _context.Books.Where(s => s.CategoryID == id);

                    if (bookList.Any())
                    {
                        return View();
                    }
                    if (!bookList.Any())
                    {
                        _context.Categories.Remove(category);
                        _context.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
            return RedirectToAction("Index","Home");
        }
        

        //-----------<Wizard generated>--------------------------------
        public IActionResult Privacy()
        {
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
