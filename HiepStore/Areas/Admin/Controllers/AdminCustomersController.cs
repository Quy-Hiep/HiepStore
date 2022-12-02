﻿using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using HiepStore.Helpper;
using HiepStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace HiepStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize]
    public class AdminCustomersController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }

        public AdminCustomersController(db_hiep_storeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;

        }

        // GET: Admin/AdminCustomers
        public IActionResult Index(int page = 1, int isActive = 2)
        {
            var pageNumber = page;
            var pageSize = 5;
            List<Customer> lsCustomers = new List<Customer>();
            if (isActive == 0)
            {
                lsCustomers = _context.Customers.AsNoTracking()
                    .Where(x => x.IsActive == false)
                    .Where(x => x.IsDeleted == false)
                    .Include(x => x.Location)
                    .OrderByDescending(x => x.CreatedAt).ToList();
            }
            else if (isActive == 1)
            {
                lsCustomers = _context.Customers.AsNoTracking()
                    .Where(x => x.IsActive == true)
                    .Where(x => x.IsDeleted == false)
                    .Include(x => x.Location)
                    .OrderByDescending(x => x.CreatedAt).ToList();
            }
            else
            {
                lsCustomers = _context.Customers.AsNoTracking()
                    .Where(x => x.IsDeleted == false)
                    .Include(x => x.Location)
                    .OrderByDescending(x => x.CreatedAt).ToList();

            }



            PagedList<Customer> models = new PagedList<Customer>(lsCustomers.AsQueryable(), pageNumber, pageSize);
            
            List<SelectListItem> lsTrangThai = new List<SelectListItem>();
            lsTrangThai.Add(new SelectListItem() { Text = "Hoạt động", Value = "1" });
            lsTrangThai.Add(new SelectListItem() { Text = "Khóa", Value = "0" });
            ViewData["lsTrangThai"] = lsTrangThai;

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        public IActionResult FiltterStatus(int IsActive = 2)
        {
            var url = $"/Admin/AdminCustomers?IsActive={IsActive}";
            if (IsActive == 2)
            {
                url = $"/Admin/AdminCustomers";
            }
            return Json(new { status = "success", redirectUrl = url });
        }
        // GET: Admin/AdminCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/AdminCustomers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCustomers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Birthday,Avatar,Address,Email,Phone,LocationId,District,Ward,CreatedAt,UpdatedAt,Password,Salt,LastLogin,IsActive,IsDeleted")] Customer customer, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(customer.FirstName) + extension;
                    customer.Avatar = await Utilities.UploadFile(fThumb, @"customers", image.ToLower());
                }
                if (string.IsNullOrEmpty(customer.Avatar)) customer.Avatar = "default.jpg";
                customer.CreatedAt = DateTime.Now;
                customer.UpdatedAt = DateTime.Now;

                _context.Add(customer);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm khách hàng thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Admin/AdminCustomers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["TinhThanh"] = new SelectList(_context.Locations, "Id", "Name", customer.LocationId);
            ViewData["QuanHuyen"] = new SelectList(_context.Districts, "Id", "Name", customer.DistrictId);
            ViewData["XaPhuong"] = new SelectList(_context.Wards, "Id", "Name", customer.WardId);
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Birthday,Avatar,Address,Email,Phone,LocationId,DistrictId,WardId,CreatedAt,UpdatedAt,Password,Salt,LastLogin,IsActive,IsDeleted")] Customer customer, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                customer.FirstName = Utilities.ToTitleCase(customer.FirstName);
                if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(customer.FirstName) + extension;
                        customer.Avatar = await Utilities.UploadFile(fThumb, @"avatars", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(customer.Avatar)) customer.Avatar = "default.jpg";
                    customer.UpdatedAt = DateTime.Now;
                    _context.Update(customer);
                    _notyfService.Success("Cập nhật thành công");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            ViewData["TinhThanh"] = new SelectList(_context.Locations, "Id", "Name", customer.LocationId);
            ViewData["QuanHuyen"] = new SelectList(_context.Districts, "Id", "Name", customer.DistrictId);
            ViewData["XaPhuong"] = new SelectList(_context.Wards, "Id", "Name", customer.WardId);

            return View(customer);
        }

        //view xem danh sách Thùng rác
        public ActionResult Trash(int page = 1)
        {
            var pageNumber = page;
            var pageSize = 5;

            List<Customer> lsCustomers = new List<Customer>();
            lsCustomers = _context.Customers
            .AsNoTracking()
            .Where(n => n.IsDeleted == true)
            .OrderByDescending(x => x.CreatedAt).ToList();

            PagedList<Customer> models = new PagedList<Customer>(lsCustomers.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        //Đưa sản phẩm vào thùng rác
        public ActionResult DeleteTrash(int id)
        {
            Customer customer = _context.Customers.Find(id);
            customer.IsDeleted = true;
            _context.Entry(customer).State = EntityState.Modified;
            _context.SaveChanges();
            _notyfService.Success("Đưa vào thùng rác thành công");
            return RedirectToAction("Index");
        }

        //khôi phục sản phẩm từ thùng rác
        public ActionResult ReTrash(int id)
        {
            Customer customer = _context.Customers.Find(id);
            customer.IsDeleted = false;
            _context.SaveChanges();
            _notyfService.Success("Khôi phục thành công");
            return RedirectToAction("Trash");
        }

        // GET: Admin/AdminCustomers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/AdminCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Trash));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}

