using BookManagerApp.Data;
using BookManagerApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BookManagerApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. READ: Danh sách sách (Index)
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }

        // 2. READ: Xem chi tiết (Details)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // 3. CREATE: Hiển thị form thêm mới (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREATE: Xử lý thêm mới kèm Upload ảnh (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Genre,Price,PublicationYear")] Book book, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                // Kiểm tra định dạng file (.jpg hoặc .png)
                var extension = Path.GetExtension(imageFile.FileName).ToLower();
                if (extension != ".jpg" && extension != ".png")
                {
                    ModelState.AddModelError("", "Chỉ chấp nhận file hình ảnh định dạng .jpg hoặc .png!");
                    return View(book);
                }

                // Tạo tên file duy nhất tránh trùng lặp
                var fileName = Guid.NewGuid().ToString() + extension;
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                book.ImageUrl = "/images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // 4. UPDATE: Hiển thị form chỉnh sửa (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // 4. UPDATE: Xử lý cập nhật (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Genre,Price,PublicationYear,ImageUrl")] Book book, IFormFile? imageFile)
        {
            if (id != book.Id) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLower();
                if (extension != ".jpg" && extension != ".png")
                {
                    ModelState.AddModelError("", "Chỉ chấp nhận file hình ảnh định dạng .jpg hoặc .png!");
                    return View(book);
                }

                var fileName = Guid.NewGuid().ToString() + extension;
                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                book.ImageUrl = "/images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // 5. DELETE: Hiển thị xác nhận xóa (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // 5. DELETE: Xử lý xóa (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}