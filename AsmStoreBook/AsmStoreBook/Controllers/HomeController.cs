using AsmStoreBook.Areas.Identity.Data;
using AsmStoreBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AsmStoreBook.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AsmStoreBookUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, UserManager<AsmStoreBookUser> userManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;

        }

        public async Task<IActionResult> Index()
        {
            var userName = await _userManager.GetUserAsync(HttpContext.User);
            /*var rolesname = await _userManager.GetRolesAsync(userName);
            if (rolesname.Contains("Customer"))
            {
                return RedirectToAction("CusIndex", "Books", new { area = "" });
            }
            if (rolesname.Contains("Seller"))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }*/
            return View();
        }
        public async Task<IActionResult> NoLogin()
        {
            var userName = await _userManager.GetUserAsync(HttpContext.User);
            return View();
        }
        public async Task<IActionResult> UserIndex()
        {
            var userName = await _userManager.GetUserAsync(HttpContext.User);
            /*var rolesname = await _userManager.GetRolesAsync(userName);
            if (rolesname.Contains("Customer"))
            {
                return RedirectToAction("CusIndex", "Books", new { area = "" });
            }
            if (rolesname.Contains("Seller"))
            {
                return RedirectToAction("Index", "Home", new { area = "" });
            }*/
            return View();
        }

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