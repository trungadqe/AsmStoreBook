#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AsmStoreBook.Areas.Identity.Data;
using AsmStoreBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AsmStoreBook.Controllers
{
    public class BooksController : Controller
    {
        private readonly AsmStoreBookContext _context;
        private readonly UserManager<AsmStoreBookUser> _userManager;
        private readonly int _numberOfRecordEachPages = 6;
        private readonly AsmStoreBookContext dbcontext;

        public BooksController(AsmStoreBookContext context, UserManager<AsmStoreBookUser> userManager, AsmStoreBookContext dbcontext)
        {
            this.dbcontext = dbcontext;
            _context = context;
            _userManager = userManager;
        }

        // GET: Books
        public async Task<IActionResult> Index(int id =0)
        {

            int numberOfRecords = await _context.Book.CountAsync();
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / _numberOfRecordEachPages);
            ViewBag.numberOfPages = numberOfPages;
            ViewBag.numberOfRecords = numberOfRecords;
            if (numberOfRecords > 0)
            {               
                int max = 5;
                int min;
                int end;
                if (numberOfRecords < max)
                {
                    min = 1;
                    end = numberOfRecords;
                }
                else
                {
                    min = id;
                    end = id + max - 1;
                }
                ViewBag.max = max;
                ViewBag.min = min;
                ViewBag.end = end;
            }
            ViewBag.EndPage = numberOfPages - 1;
            ViewBag.currentPage = id;
            List<Book> books = await _context.Book
                    .Skip(id * _numberOfRecordEachPages)
                    .Take(_numberOfRecordEachPages)
                    .Include(b => b.Category)
                    .Include(b => b.Store)
                    .ToListAsync();
            return View(books);
        }
        public async Task<IActionResult> Search(string searchString, int id = 0)
        {
            ViewData["CurrentFilter"] = searchString;
            var books = from s in dbcontext.Book
                        .Include(s => s.Category)
                        .Include(s => s.Store)
                        select s;
            if (searchString == null)
            {
            int numberOfRecords = await _context.Book.CountAsync();
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / _numberOfRecordEachPages);
            ViewBag.numberOfPages = numberOfPages;
            ViewBag.numberOfRecords = numberOfRecords;
            if (numberOfRecords > 0)
            {               
                int max = 5;
                int min;
                int end;
                if (numberOfRecords < max)
                {
                    min = 1;
                    end = numberOfRecords;
                }
                else
                {
                    min = id;
                    end = id + max - 1;
                }
                ViewBag.max = max;
                ViewBag.min = min;
                ViewBag.end = end;
            }
            ViewBag.EndPage = numberOfPages - 1;
            ViewBag.currentPage = id;
            List<Book> booksList = await _context.Book
                    .Skip(id * _numberOfRecordEachPages)
                    .Take(_numberOfRecordEachPages)
                    .Include(b => b.Category)
                    .Include(b => b.Store)
                    .ToListAsync();
            return View(books);
            }
            else
            {
                books = books.Where(s => s.Title.Contains(searchString));
                int numberOfRecords = books.Count();
                int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / _numberOfRecordEachPages);
                ViewBag.numberOfPages = numberOfPages;
                ViewBag.numberOfRecords = numberOfRecords;
                if (numberOfRecords > 0)
                {
                    int max = 5;
                    int min;
                    int end;
                    if (numberOfRecords < max)
                    {
                        min = 1;
                        end = numberOfRecords;
                    }
                    else
                    {
                        min = id;
                        end = id + max - 1;
                        if (end > numberOfRecords)
                        {
                            end = numberOfRecords;
                        }
                    }
                    ViewBag.max = max;
                    ViewBag.min = min;
                    ViewBag.end = end;
                }
                List<Book> booksList = await books
                    .Skip(id * _numberOfRecordEachPages)
                    .Take(_numberOfRecordEachPages)
                    .Include(b => b.Category)
                    .Include(b => b.Store)
                    .ToListAsync();
                ViewBag.EndPage = numberOfPages - 1;
                ViewBag.currentPage = id;
                return View(books);
            }            
            return View();
        }
        /*public async Task<IActionResult> ListBookOfStore(int? id)
        {
            var books = from s in dbcontext.Book
                       .Include(s => s.Category)
                       .Include(s => s.Store)
                        select s;
            books = books.Where(s => s.Store.Id.Contains(id));
            int numberOfRecords = books.Count();
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / _numberOfRecordEachPages);
            ViewBag.numberOfPages = numberOfPages;
            ViewBag.numberOfRecords = numberOfRecords;
            if (numberOfRecords > 0)
            {
                int max = 5;
                int min;
                int end;
                if (numberOfRecords < max)
                {
                    min = 1;
                    end = numberOfRecords;
                }
                else
                {
                    min = id;
                    end = id + max - 1;
                }
                ViewBag.max = max;
                ViewBag.min = min;
                ViewBag.end = end;
            }
            List<Book> booksList = await books
                .Skip(id * _numberOfRecordEachPages)
                .Take(_numberOfRecordEachPages)
                .Include(b => b.Category)
                .Include(b => b.Store)
                .ToListAsync();
            ViewBag.EndPage = numberOfPages - 1;
            ViewBag.currentPage = id;
            return View(books);
        }*/
        public async Task<IActionResult> UserIndexAsync()
        {
            var Book = _context.Book.Include(b => b.Store)
                                    .Include(b => b.Category);
            ViewData["CategoryName"] = new SelectList(_context.Category.ToList(), "Name", "Name");
            return View(await Book.ToListAsync());
        }
        // GET: Books/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Category)
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            var store = await _context.Store.FirstAsync(x => x.UId == userId);

            if (store == null)
            {
                return RedirectToAction("Create", "Stores", new { area = "" });
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id");
            ViewData["StoreID"] = new SelectList(_context.Store.Where(c => c.Id == store.Id), "Id", "Id");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,Title,Pages,Author,Price,Desc,ImgUrl,StoreId,CategoryId")] Book book, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string imgName = book.Isbn + Path.GetExtension(image.FileName);
                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        image.CopyTo(stream);
                    }
                    book.ImgUrl = "/img/" + imgName;
                    var userId = _userManager.GetUserId(HttpContext.User);

                    var store = await _context.Store.FirstAsync(x => x.UId == userId);

                    if (store == null)
                    {
                        return RedirectToAction("Create", "Stores", new { area = "" });
                    } 
                }
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", book.CategoryId);
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id", book.StoreId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            //check Owner
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                                .Include(b => b.Store)
                                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", book.CategoryId);
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id", book.StoreId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Isbn,Title,Pages,Author,Price,Desc,ImgUrl,StoreId,CategoryId")] Book book)
        {
            if (id != book.Isbn)
            {
                return NotFound();
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
                    if (!BookExists(book.Isbn))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Id", book.CategoryId);
            ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id", book.StoreId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Category)
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string id)
        {
            return _context.Book.Any(e => e.Isbn == id);
        }
    }
}
