using AspNetCoreHero.ToastNotification.Abstractions;
using HiepStore.Areas.Admin.Models;
using HiepStore.Helpper;
using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Security.Principal;

namespace HiepStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountsController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }
        public AdminAccountsController(db_hiep_storeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

		// GET: Admin/AdminAccounts
		public IActionResult Index(int page = 1, int isActive = 2)
		{
			var pageNumber = page;
			var pageSize = 5;
			List<Account> lsAccounts = new List<Account>();
			if (isActive == 0)
			{
				lsAccounts = _context.Accounts.AsNoTracking()
					.Where(x => x.IsActive == false)
					.Where(x => x.IsDeleted == false)
					.OrderByDescending(x => x.CreatedAt).ToList();
			}
			else if (isActive == 1)
			{
				lsAccounts = _context.Accounts.AsNoTracking()
					.Where(x => x.IsActive == true)
					.Where(x => x.IsDeleted == false)
					.OrderByDescending(x => x.CreatedAt).ToList();
			}
			else
			{
				lsAccounts = _context.Accounts.AsNoTracking()
					.Where(x => x.IsDeleted == false)
					.OrderByDescending(x => x.CreatedAt).ToList();
			}

			PagedList<Account> models = new PagedList<Account>(lsAccounts.AsQueryable(), pageNumber, pageSize);

			List<SelectListItem> lsTrangThai = new List<SelectListItem>();
			lsTrangThai.Add(new SelectListItem() { Text = "Hoạt động", Value = "1" });
			lsTrangThai.Add(new SelectListItem() { Text = "Khóa", Value = "0" });
			ViewData["lsTrangThai"] = lsTrangThai;
			ViewBag.CurrentPage = pageNumber;
			return View(models);
		}

		public IActionResult FiltterStatus(int IsActive = 2)
		{
			var url = $"/Admin/AdminAccounts?IsActive={IsActive}";
			if (IsActive == 2)
			{
				url = $"/Admin/AdminAccounts";
			}
			return Json(new { status = "success", redirectUrl = url });
		}
		// GET: Admin/AdminAccounts/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var Account = await _context.Accounts
				.FirstOrDefaultAsync(m => m.Id == id);
			if (Account == null)
			{
				return NotFound();
			}

			return View(Account);
		}

		// GET: Admin/AdminAccounts/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Admin/AdminAccounts/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AccountViewModel AccountView, Microsoft.AspNetCore.Http.IFormFile fThumb)
		{
			try
			{
				Account account = new Account();
				account.FirstName = AccountView.FirstName;
				account.LastName = AccountView.LastName;
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(AccountView.LastName + AccountView.FirstName) + extension;
                    account.Avatar = await Utilities.UploadFile(fThumb, @"Avatars", image.ToLower());
                }
                if (string.IsNullOrEmpty(account.Avatar)) account.Avatar = "default.jpg";
				account.Email = AccountView.Email;
				account.Phone = AccountView.Phone;
				account.Password = AccountView.Password;
				account.IsActive = AccountView.IsActive;
				account.IsDeleted = false;
				account.RoleId = 1;
                account.CreatedAt = DateTime.Now;

                _context.Add(account);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm khách hàng thành công");
                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
			{
                return View(AccountView);

            }
		}

		// GET: Admin/AdminAccounts/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            AccountViewModel accountView = new AccountViewModel();
			accountView.Id = account.Id;
			accountView.FirstName = account.FirstName;
			accountView.LastName = account.LastName;
			accountView.Email = account.Email;
			accountView.Phone = account.Phone;
			accountView.Avatar = account.Avatar;
			accountView.CreatedAt = account.CreatedAt;
			accountView.UpdatedAt = account.UpdatedAt;
			accountView.IsActive = account.IsActive;
			accountView.IsDeleted = account.IsDeleted;
			accountView.RoleId = account.RoleId;

            
			return View(accountView);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, AccountViewModel AccountView, Microsoft.AspNetCore.Http.IFormFile fThumb)
		{
			if (id != AccountView.Id)
			{
				return NotFound();
			}
            var account = await _context.Accounts.FindAsync(id);

            try
            {
                account.FirstName = Utilities.ToTitleCase(AccountView.FirstName);
                account.LastName = Utilities.ToTitleCase(AccountView.LastName);
				if (fThumb != null)
				{
					string extension = Path.GetExtension(fThumb.FileName);
					string image = Utilities.SEOUrl(account.LastName + account.FirstName) + extension;
                    account.Avatar = await Utilities.UploadFile(fThumb, @"avatars", image.ToLower());
				}
				if (string.IsNullOrEmpty(account.Avatar)) account.Avatar = "default.jpg";
                account.Email = AccountView.Email;
                account.Phone = AccountView.Phone;
                account.IsActive = AccountView.IsActive;
                account.IsDeleted = AccountView.IsDeleted;
                account.RoleId = 1;
                account.UpdatedAt = DateTime.Now;
				_context.Update(account);
				await _context.SaveChangesAsync();
                _notyfService.Success("Cập nhật thành công");
            }
            catch (DbUpdateConcurrencyException)
			{
				if (!AccountExists(account.Id))
				{
					return NotFound();
				}
				else
				{
					return View(AccountView);
				}
			}
			return RedirectToAction(nameof(Index));

		}

		//view xem danh sách Thùng rác
		public ActionResult Trash(int page = 1)
		{
			var pageNumber = page;
			var pageSize = 5;

			List<Account> lsAccounts = new List<Account>();
			lsAccounts = _context.Accounts
			.AsNoTracking()
			.Where(n => n.IsDeleted == true)
			.OrderByDescending(x => x.CreatedAt).ToList();

			PagedList<Account> models = new PagedList<Account>(lsAccounts.AsQueryable(), pageNumber, pageSize);
			ViewBag.CurrentPage = pageNumber;
			return View(models);
		}

		//Đưa sản phẩm vào thùng rác
		public ActionResult DeleteTrash(int id)
		{
			Account Account = _context.Accounts.Find(id);
			Account.IsDeleted = true;
			_context.Entry(Account).State = EntityState.Modified;
			_context.SaveChanges();
			_notyfService.Success("Đưa vào thùng rác thành công");
			return RedirectToAction("Index");
		}

		//khôi phục sản phẩm từ thùng rác
		public ActionResult ReTrash(int id)
		{
			Account Account = _context.Accounts.Find(id);
			Account.IsDeleted = false;
			_context.SaveChanges();
			_notyfService.Success("Khôi phục thành công");
			return RedirectToAction("Trash");
		}

		// GET: Admin/AdminAccounts/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var Account = await _context.Accounts
				.FirstOrDefaultAsync(m => m.Id == id);
			if (Account == null)
			{
				return NotFound();
			}

			return View(Account);
		}

		// POST: Admin/AdminAccounts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var Account = await _context.Accounts.FindAsync(id);
			_context.Accounts.Remove(Account);
			await _context.SaveChangesAsync();
			_notyfService.Success("Xóa thành công");
			return RedirectToAction(nameof(Trash));
		}

		private bool AccountExists(int id)
		{
			return _context.Accounts.Any(e => e.Id == id);
		}
	}
}
