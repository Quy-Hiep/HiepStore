using AspNetCoreHero.ToastNotification.Abstractions;
using HiepStore.Helpper;
using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Drawing.Drawing2D;

namespace HiepStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBrandsController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }
        public AdminBrandsController(db_hiep_storeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminCategories
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var lsBrands = _context.Brands
                .AsNoTracking()
                .OrderBy(x => x.Id);
            PagedList<Brand> models = new PagedList<Brand>(lsBrands, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Admin/AdminCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Brand brand, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            try
            {
				if (fThumb != null)
				{
					string extension = Path.GetExtension(fThumb.FileName);
					string imageName = Utilities.SEOUrl(brand.Name) + extension;
					brand.Thumb = await Utilities.UploadFile(fThumb, @"brand", imageName.ToLower());
				}
				if (string.IsNullOrEmpty(brand.Thumb)) brand.Thumb = "default.jpg";
				brand.Alias = Utilities.SEOUrl(brand.Name);
				_context.Add(brand);
				await _context.SaveChangesAsync();
				_notyfService.Success("Thêm mới thành công");
				return RedirectToAction(nameof(Index));

			}
			catch (Exception)
            {

				return View(brand);
			}
			//Xu ly Thumb
		}

        // GET: Admin/AdminCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Admin/AdminCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Brand brand, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }

            try
            {
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string imageName = Utilities.SEOUrl(brand.Name) + extension;
                    brand.Thumb = await Utilities.UploadFile(fThumb, @"brand", imageName.ToLower());
                }
                if (string.IsNullOrEmpty(brand.Thumb)) brand.Thumb = "default.jpg";
                _context.Update(brand);
                await _context.SaveChangesAsync();
                _notyfService.Success("Chỉnh sửa thành công");
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(brand.Id))
                {
                    return NotFound();
                }
                else
                {
				    return View(brand);
			    }
		    }
        }

        // GET: Admin/AdminCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // POST: Admin/AdminCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}
