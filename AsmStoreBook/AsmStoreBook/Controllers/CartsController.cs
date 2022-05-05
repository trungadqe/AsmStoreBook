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

namespace AsmStoreBook.Controllers
{
    public class CartsController : Controller
    {
        private readonly AsmStoreBookContext _context;
        private readonly UserManager<AsmStoreBookUser> _userManager;
        public CartsController(AsmStoreBookContext context, UserManager<AsmStoreBookUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            if (thisUserId == null)
            {
                return RedirectToAction("NoLogin", "Home");
            }
            var cart = _context.Cart
                .Include(c => c.Book)
                .Where(c => c.UId == thisUserId);
            /*foreach (var item in cart)
            {
                var Total = item.Quantity * item.Book.Price;
                var All = Total;
            }*/
            /*ViewData["All"] = All;
             nghiên cứu thêm lỗi chỗ ni */
            /*ViewData["TotalPrice"] = ne*/
            ViewData["subPrice"] = new SelectList(_context.Cart.Where(c => c.UId == thisUserId), "TotalPrice", "TotalPrice");
            /*Cart subPrice = cart.Take();
            Book test = Book.Take()*/
            return View(cart);
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public async Task<IActionResult> AddToCart(string isbn, double Price, [Bind("Quantity")] Cart cart)
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            if (thisUserId == null)
            {
                return RedirectToAction("NoLogin", "Home");
            }
            Cart myCart = new Cart()
            {
                UId = thisUserId,
                BookIsbn = isbn,
                Quantity = 1,
                UnitPrice = Price,
                TotalPrice = Price,
            };
            Cart fromDb = _context.Cart.FirstOrDefault(c => c.UId == thisUserId && c.BookIsbn == isbn);
            if (fromDb == null)
            {
                _context.Add(myCart);
                await _context.SaveChangesAsync();
            }
            else
            {
                fromDb.Quantity++;
                fromDb.UnitPrice = Price * fromDb.Quantity;
                fromDb.TotalPrice = fromDb.TotalPrice + Price;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Books");
        }
        public async Task<IActionResult> IncreaseNumBook(string isbn, double Price, double Quantity)
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            Cart myCart = new Cart()
            {
                UId = thisUserId,
                BookIsbn = isbn,
                Quantity = 1,
                UnitPrice = Price,
                TotalPrice = Price,
            };
            Cart fromDb = _context.Cart.FirstOrDefault(c => c.UId == thisUserId && c.BookIsbn == isbn);
            if (fromDb == null)
            {
                _context.Add(myCart);
                await _context.SaveChangesAsync();
            }
            else
            {
                fromDb.Quantity = Quantity;
                fromDb.UnitPrice = Price * fromDb.Quantity;
                fromDb.TotalPrice = fromDb.TotalPrice + fromDb.Quantity;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Carts");
        }
        public async Task<IActionResult> Checkout()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            List<Cart> myDetailsInCart = await _context.Cart
                .Where(c => c.UId == thisUserId)
                .Include(c => c.Book)
                .ToListAsync();
            if (myDetailsInCart.Count > 0)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        //Step 1: create an order
                        Order myOrder = new Order();
                        myOrder.UId = thisUserId;
                        myOrder.OrderDate = DateTime.Now;
                        /*myOrder.Total = myDetailsInCart.Select(c => c.Book.Price)
                            .Aggregate((c1, c2) => c1 + c2);
                        _context.Add(myOrder);*/
                        myOrder.Total = myDetailsInCart.Select(c => c.UnitPrice)
                            .Aggregate((c1, c2) => c1 + c2);
                        _context.Add(myOrder);
                        await _context.SaveChangesAsync();

                        //Step 2: insert all order details by var "myDetailsInCart"
                        foreach (var item in myDetailsInCart)
                        {
                            OrderDetail detail = new OrderDetail()
                            {
                                OrderId = myOrder.Id,
                                BookIsbn = item.BookIsbn,
                                Quantity = item.Quantity,
                            };
                            _context.Add(detail);
                        }
                        await _context.SaveChangesAsync();

                        //Step 3: empty/delete the cart we just done for thisUser
                        _context.Cart.RemoveRange(myDetailsInCart);
                        await _context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    catch (DbUpdateException ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error occurred in Checkout" + ex);
                    }
                }
            }
            else
            {
                return RedirectToAction("Index", "Books");
            }

            return RedirectToAction("Index", "Carts");
        }


        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UId,BookIsbn")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookIsbn"] = new SelectList(_context.Book, "Isbn", "Isbn", cart.BookIsbn);
            ViewData["UId"] = new SelectList(_context.Users, "Id", "Id", cart.UId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["BookIsbn"] = new SelectList(_context.Book, "Isbn", "Isbn", cart.BookIsbn);
            ViewData["UId"] = new SelectList(_context.Users, "Id", "Id", cart.UId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UId,BookIsbn")] Cart cart)
        {
            if (id != cart.UId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.UId))
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
            ViewData["BookIsbn"] = new SelectList(_context.Book, "Isbn", "Isbn", cart.BookIsbn);
            ViewData["UId"] = new SelectList(_context.Users, "Id", "Id", cart.UId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string BookId)
        {
            if (BookId == null)
            {
                return NotFound();
            }
            var userId = _userManager.GetUserId(HttpContext.User);
            var myCart = await _context.Cart
                .Include(c => c.User)
                .Where(c => c.UId == userId) 
                .FirstOrDefaultAsync(c => c.BookIsbn == BookId);
            if (myCart == null)
            {
                return NotFound();      
             }

            return View(myCart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string BookId)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var cart = await _context.Cart
                .Include(c => c.User)
                .Where(c => c.UId == userId)
                .FirstOrDefaultAsync(c => c.BookIsbn == BookId);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(string id)
        {
            return _context.Cart.Any(e => e.UId == id);
        }
    }   
}
