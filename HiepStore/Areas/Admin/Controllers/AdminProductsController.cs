using AspNetCoreHero.ToastNotification.Abstractions;
using HiepStore.Areas.Admin.Models;
using HiepStore.Helpper;
using HiepStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace HiepStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminProductsController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }

        public AdminProductsController(db_hiep_storeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;

        }

        // GET: Admin/AdminProducts
        public async Task<IActionResult> Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 10;

            List<Product> lsProducts = new List<Product>();

            if (CatID != 0)
            {
                lsProducts = _context.Products
                .AsNoTracking()
                .Where(x => x.CategoryId == CatID)
                .Where(x => x.IsDeleted == false)
                .Include(x => x.Category)
                .OrderBy(x => x.Id).ToList();
            }
            else
            {
                lsProducts = _context.Products
                .AsNoTracking()
                .Where(x => x.IsDeleted == false)
                .Include(x => x.Category)
                .OrderBy(x => x.Id).ToList();
            }


            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;

            ViewBag.CurrentPage = pageNumber;

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Id", "Name");

            return View(models);
        }
        public IActionResult FiltterCat(int CatID = 0)
        {
            var url = $"/Admin/AdminProducts?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/AdminProducts";
            }
            return Json(new { status = "success", redirectUrl = url });
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["LoaiSanPham"] = new SelectList(_context.ProductTypes, "Id", "Name");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productView, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            try
            {
                Product product = new Product();
                product.Name = Utilities.ToTitleCase(productView.Name);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.Name) + extension;
                    product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }
                if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
                product.Alias = Utilities.SEOUrl(product.Name);
                product.ShortDesc = productView.ShortDesc;
                product.Description = productView.Description;
                product.CategoryId = productView.CategoryId;
                product.BrandId = productView.BrandId;
                product.TypeId = productView.TypeId;
                product.UnitsInStock = productView.UnitsInStock;
                product.Price = productView.Price;
                product.Discount = productView.Discount;
                product.IsActive = productView.IsActive;
                product.IsShowOnHomePage = productView.IsShowOnHomePage;
                product.IsDeleted = productView.IsDeleted;
                product.Title = productView.Title;
                product.MetaDesc = productView.MetaDesc;
                product.MetaDesc = productView.MetaDesc;
                product.Tags = productView.Tags;
                product.CreatedAt = DateTime.Now;
                product.UpdatedAt = DateTime.Now;

                _context.Add(product);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm mới thành công");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewData["DanhMuc"] = new SelectList(_context.Categories, "Id", "Name", productView.CategoryId);
                ViewData["ThuongHieu"] = new SelectList(_context.Brands, "Id", "Name", productView.BrandId);
                ViewData["LoaiSanPham"] = new SelectList(_context.ProductTypes, "Id", "Name", productView.TypeId);
                return View(productView);

            }
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            var productView = new ProductViewModel();
            productView.Id = product.Id;
            productView.Name = product.Name;
            productView.ShortDesc = product.ShortDesc;
            productView.Description = product.Description;
            productView.CategoryId = product.CategoryId;
            productView.BrandId = product.BrandId;
            productView.Price = product.Price;
            productView.Discount = product.Discount;
            productView.UnitsInStock = product.UnitsInStock;
            productView.Thumb = product.Thumb;
            productView.Video = product.Video;
            productView.TypeId = product.TypeId;
            productView.Ordering = product.Ordering;
            productView.IsActive = product.IsActive;
            productView.IsDeleted = product.IsDeleted;
            productView.CreatedAt = product.CreatedAt;
            productView.UpdatedAt = product.UpdatedAt;
            productView.UserCreated = product.UserCreated;
            productView.UserUpdated = product.UserUpdated;
            productView.Tags = product.Tags;
            productView.Title = product.Title;
            productView.Alias = product.Alias;
            productView.MetaDesc = product.MetaDesc;
            productView.MetaKey = product.MetaKey;
            productView.Configuration = product.Configuration;
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ThuongHieu"] = new SelectList(_context.Brands, "Id", "Name", productView.BrandId);
            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Id", "Name", productView.CategoryId);
            ViewData["LoaiSanPham"] = new SelectList(_context.ProductTypes, "Id", "Name", productView.TypeId);
            return View(productView);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel productView, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            //if (id != product.Id)
            //{
            //    return NotFound();
            //}

            try
            {
                var product = await _context.Products.FindAsync(id);
                product.Name = productView.Name;
                product.ShortDesc = productView.ShortDesc;
                product.Description = productView.Description;
                product.CategoryId = productView.CategoryId;
                product.BrandId = productView.BrandId;
                product.Price = productView.Price;
                product.Discount = productView.Discount;
                product.UnitsInStock = productView.UnitsInStock;
                product.Video = productView.Video;
                product.TypeId = productView.TypeId;
                product.Ordering = productView.Ordering;
                product.IsActive = productView.IsActive;
                product.IsDeleted = productView.IsDeleted;
                product.CreatedAt = productView.CreatedAt;
                product.UserCreated = productView.UserCreated;
                product.Tags = productView.Tags;
                product.Title = productView.Title;
                product.Alias = productView.Alias;
                product.MetaDesc = productView.MetaDesc;
                product.MetaKey = productView.MetaKey;
                product.Configuration = productView.Configuration;


                product.Name = Utilities.ToTitleCase(product.Name);
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.Name) + extension;
                    product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }
                if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
                product.Alias = Utilities.SEOUrl(product.Name);
                product.UpdatedAt = DateTime.Now;

                _context.Update(product);
                _notyfService.Success("Cập nhật thành công");
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(productView.Id))
                {
                    return NotFound();
                }
                else
                {
                    return View(productView);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        //view xem danh sách Thùng rác
        public ActionResult Trash(int page = 1, int CatID = 0)
        {
            var pageNumber = page;
            var pageSize = 10;

            List<Product> lsProducts = new List<Product>();
            if (CatID != 0)
            {
                lsProducts = _context.Products
                .AsNoTracking()
                .Where(n => n.IsDeleted == true)
                .Where(x => x.CategoryId == CatID)
                .Include(x => x.Category)
                .OrderBy(x => x.Id).ToList();
            }
            else
            {
                lsProducts = _context.Products
                .AsNoTracking()
                .Where(n => n.IsDeleted == true)
                .Include(x => x.Category)
                .OrderBy(x => x.Id).ToList();
            }


            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;

            ViewBag.CurrentPage = pageNumber;

            ViewData["DanhMuc"] = new SelectList(_context.Categories, "Id", "Name");

            return View(models);
        }

        //Đưa sản phẩm vào thùng rác
        public ActionResult DeleteTrash(int id)
        {
            Product product = _context.Products.Find(id);
            product.IsDeleted = true;
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            _notyfService.Success("Đưa vào thùng rác thành công");
            return RedirectToAction("Index");
        }

        //khôi phục sản phẩm từ thùng rác
        public ActionResult ReTrash(int Id)
        {
            Product product = _context.Products.Find(Id);
            product.IsDeleted = false;
            _context.SaveChanges();
            _notyfService.Success("Khôi phục thành công");
            return RedirectToAction("Trash");
        }


        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'db_hiep_storeContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Trash));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

