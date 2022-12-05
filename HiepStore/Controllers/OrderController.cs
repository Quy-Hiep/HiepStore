using AspNetCoreHero.ToastNotification.Abstractions;
using HiepStore.Models;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiepStore.Controllers
{
	public class OrderController : Controller
	{
		private readonly db_hiep_storeContext _context;
		public INotyfService _notyfService { get; }
		public OrderController(db_hiep_storeContext context, INotyfService notyfService)
		{
			_context = context;
			_notyfService = notyfService;
		}
		[HttpPost]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			try
			{
				var taikhoanID = HttpContext.Session.GetString("CustomerId");
				if (string.IsNullOrEmpty(taikhoanID)) return RedirectToAction("Login", "Accounts");
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(taikhoanID));
				if (khachhang == null) return NotFound();
				var donhang = await _context.Orders
					.Include(x => x.TransactStatus)
					.FirstOrDefaultAsync(m => m.Id == id && Convert.ToInt32(taikhoanID) == m.CustomerId);
				if (donhang == null) return NotFound();

				var chitietdonhang = _context.OrderDetails
					.Include(x => x.Product)
					.AsNoTracking()
					.Where(x => x.OrderId == id)
					.OrderBy(x => x.Id)
					.ToList();
				XemDonHang donHang = new XemDonHang();
				donHang.DonHang = donhang;
				donHang.ChiTietDonHang = chitietdonhang;
				return PartialView("Details", donHang);

			}
			catch
			{
				return NotFound();
			}
		}
	}
}
