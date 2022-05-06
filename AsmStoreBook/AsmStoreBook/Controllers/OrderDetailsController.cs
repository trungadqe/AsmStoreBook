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

namespace AsmStoreBook.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly AsmStoreBookContext _context;

        public OrderDetailsController(AsmStoreBookContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index(int StoreId)
        {          
            List<OrderDetail> orderDetail = await _context.OrderDetail
                .Include(o => o.Book)
                .ThenInclude(o => o.Store)
                .Where(o => o.Book.StoreId == StoreId)
                .Include(o => o.Order)
                .ThenInclude(o => o.User)
                .ToListAsync();      
            return View(orderDetail);
        }
        private bool OrderDetailExists(int? id)
        {
            return _context.OrderDetail.Any(e => e.OrderId == id);
        }
    }
}
