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
            double? totalPrice;
            List<Cart> myDetailsInCart = await _context.Cart
                .Where(c => c.UId == thisUserId)
                .ToListAsync();
            if (myDetailsInCart.Count == 0)
            {
                totalPrice = 0;
            }
            else
            {
                totalPrice = myDetailsInCart
                            .Select(m => m.UnitPrice)
                            .Aggregate((c1, c2) => c1 + c2);
            }                      
            int countProduct = myDetailsInCart.Count;
            ViewData["countProduct"] = countProduct;
            ViewData["totalPrice"] = totalPrice;
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(string isbn, double Price)
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
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Books");
        }
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]        
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
        // GET: Carts/Delete/5

        public async Task<IActionResult> Delete(string BookIsbn)
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            var ItemCart = await _context.Cart
                .Where(c => c.UId == thisUserId)
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.BookIsbn == BookIsbn);
            if (ItemCart != null)
            {
                _context.Cart.Remove(ItemCart);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Carts");
        }
        public async Task<IActionResult> Confirm(string BookIsbn, int quantity)
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            var ItemCart = await _context.Cart
                .Where(c => c.UId == thisUserId)
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.BookIsbn == BookIsbn);
            if (ItemCart != null)
            {
                ItemCart.Quantity = quantity;
                ItemCart.UnitPrice = ItemCart.Book.Price * quantity;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Carts");
        }

        private bool CartExists(string id)
        {
            return _context.Cart.Any(e => e.UId == id);
        }
    }
}
