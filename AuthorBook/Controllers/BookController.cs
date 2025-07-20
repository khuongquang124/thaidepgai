using AuthorBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace AuthorBook.Controllers
{
    public class BookController : Controller
    {
        private readonly BookAuthorDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public BookController(BookAuthorDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Author) // Đảm bảo load cả thông tin tác giả
                .OrderBy(b => b.Title)  // Sắp xếp cho dễ kiểm tra
                .ToListAsync();

            Console.WriteLine($"Found {books.Count} books"); // Debug log
            return View(books);
        }

        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book, IFormFile coverImage)
        {
            Console.WriteLine("Create POST started"); // Debug log

            if (ModelState.IsValid)
            {
                try
                {
                    // Xử lý ảnh (nếu có)
                    if (coverImage != null && coverImage.Length > 0)
                    {
                        // ... code upload ảnh giữ nguyên ...
                    }
                    else
                    {
                        Console.WriteLine("No cover image uploaded"); // Debug log
                    }

                    _context.Add(book);
                    int recordsAffected = await _context.SaveChangesAsync();
                    Console.WriteLine($"Saved {recordsAffected} records"); // Debug log

                    TempData["Message"] = "Thêm sách thành công!"; // Thông báo
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}"); // Debug log
                    ModelState.AddModelError("", "Lỗi khi lưu sách: " + ex.Message);
                }
            }
            else
            {
                // Log lỗi validation
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
            }

            await PopulateAuthorsDropDownList(book.AuthorId);
            return View(book);
        }

        // Các action khác (Edit, Delete) giữ nguyên...

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }

        private async Task PopulateAuthorsDropDownList(object selectedAuthor = null)
        {
            var authorsQuery = await _context.Authors.ToListAsync();
            ViewBag.AuthorId = new SelectList(authorsQuery, "AuthorId", "Name", selectedAuthor);
        }
    }
}