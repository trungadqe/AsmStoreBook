﻿#nullable disable
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
            return View(_context.Cart.Where(c => c.UId == thisUserId));
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
                .FirstOrDefaultAsync(m => m.UId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public async Task<IActionResult> AddToCart(string isbn)
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            Cart myCart = new Cart() { UId = thisUserId, BookIsbn = isbn };
            Cart fromDb = _context.Cart.FirstOrDefault(c => c.UId == thisUserId && c.BookIsbn == isbn);
            //if not existing (or null), add it to cart. If already added to Cart before, ignore it.
            if (fromDb == null)
            {
                _context.Add(myCart);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Checkout()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            List<Cart> myDetailsInCart = await _context.Cart
                .Where(c => c.UId == thisUserId)
                .Include(c => c.Book)
                .ToListAsync();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //Step 1: create an order
                    Order myOrder = new Order();
                    myOrder.UId = thisUserId;
                    myOrder.OrderDate = DateTime.Now;
                    myOrder.Total = myDetailsInCart.Select(c => c.Book.Price)
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
                            Quantity = 1
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
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cart = await _context.Cart.FindAsync(id);
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
