using AsmStoreBook.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AsmStoreBook.Controllers
{
    public class UsersController : Controller
    {
        private readonly AsmStoreBookContext _context;
        private readonly UserManager<AsmStoreBookUser> _userManager;
        public UsersController(AsmStoreBookContext context, UserManager<AsmStoreBookUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        /*public async Task<IActionResult> Edit()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _context.Users
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("DoB,FullName,PhoneNumber,Address")] AsmStoreBookUser user)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsmStoreBookUserExists(user.Id))
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
            *//*ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id", book.StoreId);*//*
            return View(user);
        }*/
        /*private bool AsmStoreBookUserExists(int? id)
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            return _context.Users.Any(u => u.Id == userId);

        }*/
    }
}
