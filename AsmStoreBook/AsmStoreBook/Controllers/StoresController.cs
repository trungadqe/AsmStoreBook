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
    public class StoresController : Controller
    {
        private readonly AsmStoreBookContext _context;
        private readonly UserManager<AsmStoreBookUser> _userManager;
        public StoresController(AsmStoreBookContext context, UserManager<AsmStoreBookUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Stores
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Index()
        {
            var userName = await _userManager.GetUserAsync(HttpContext.User);
            if (userName == null)
            {
                return View(await _context.Store
                    .Include(s => s.User)
                    .ToListAsync());
            }
            var rolesname = await _userManager.GetRolesAsync(userName);
            var userId = _userManager.GetUserId(HttpContext.User);
            var store = _context.Store.FirstOrDefault(x => x.UId == userId);
            if (store == null)
            {
                return RedirectToAction("Create", "Stores", new { area = "" });
            }
            var asmStoreBookContext = _context.Store.Include(s => s.User);
            return RedirectToAction("Details", "Stores", new { area = "" });
        }
        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            var userId = _userManager.GetUserId(HttpContext.User);
            if (userId == null)
            {
                return RedirectToAction("NoLogin", "Home");
            }
            var userName = await _userManager.GetUserAsync(HttpContext.User);
            var rolesname = await _userManager.GetRolesAsync(userName);
            if (rolesname.Contains("Seller"))
            {
                var store = _context.Store
                        .Include(s => s.User)
                        .FirstOrDefault(x => x.UId == userId);
                id = store.Id;
                if (id == null)
                {
                    return NotFound();
                }
                if (store == null)
                {
                    return NotFound();
                }
                return View(store);
            }
            if (rolesname.Contains("Customer"))
            {
                var store = await _context.Store
                           .FirstOrDefaultAsync(m => m.Id == id);
                if (id == null)
                {
                    return NotFound();
                }
                if (store == null)
                {
                    return NotFound();
                }
                return View(store);
            }

            return View();
        }

        // GET: Stores/Create
        [Authorize(Roles = "Seller")]
        public IActionResult Create()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            ViewData["UId"] = new SelectList(_context.Users.Where(c => c.Id == userId), "Id", "Id");
            return View();
        }

        // POST: Stores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Slogan,UId")] Store store)
        {
            if (ModelState.IsValid)
            {
                _context.Add(store);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var userId = _userManager.GetUserId(HttpContext.User);
            ViewData["Uid"] = new SelectList(_context.Users.Where(c => c.Id == userId), "Id", "Id");
            return RedirectToAction("Index", "Stores", new { area = "" });
        }

        // GET: Stores/Edit/5
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var store = await _context.Store.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            ViewData["UId"] = new SelectList(_context.Users, "Id", "Id", store.UId);
            return View(store);
        }

        // POST: Stores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Address,Slogan,UId")] Store store)
        {
            if (id != store.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(store);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreExists(store.Id))
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
            ViewData["UId"] = new SelectList(_context.Users, "Id", "Id", store.UId);
            return View(store);
        }

        // GET: Stores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var store = await _context.Store
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var store = await _context.Store.FindAsync(id);
            _context.Store.Remove(store);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StoreExists(int? id)
        {
            return _context.Store.Any(e => e.Id == id);
        }
    }
}
