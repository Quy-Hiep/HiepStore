using AspNetCoreHero.ToastNotification.Abstractions;
using HiepStore.Areas.Admin.Models;
using HiepStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiepStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin.html", Name = "AdminIndex")]
    public class HomeController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }
        public HomeController(db_hiep_storeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public IActionResult Index()
        {
            var strSession = HttpContext.Session.GetString("AccountId");
            //nếu Session đăng nhập bằng null thì gọi trang đăng nhập kèm return url
            if (strSession == null)
            {
                string returnUrl = "/admin.html";
                return Redirect("/admin-login.html?returnUrl="+ System.Net.WebUtility.UrlEncode(returnUrl));

            }
            DasboardViewModel dasboardView = new DasboardViewModel();

            dasboardView.ListProduct = _context.Products.AsNoTracking()
                .Include(x=>x.Category)
                .Where(x => x.IsActive == true && x.IsDeleted == false)
                .OrderByDescending(x => x.CreatedAt).ToList();

            dasboardView.ListOrder = _context.Orders.AsNoTracking()
                .Include(x=>x.OrderDetails)
                .Include(x=>x.TransactStatus)
                .Include(x=>x.Customer)
                .OrderByDescending(x =>x.Id).ToList();
            return View(dasboardView);
        }
    }
}
