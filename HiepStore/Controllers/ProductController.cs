using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace HiepStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly db_hiep_storeContext _context;

        public ProductController(db_hiep_storeContext context)
        {
            _context = context;
        }

        [Route("/{Alias}-{id}.html", Name = ("ProductDetails"))]
        public IActionResult Detail(int Id)
        {
            var objProduct = _context.Products
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Where(n => n.Id == Id).FirstOrDefault();
            
            ViewBag.lstPost = _context.News.Where(n => n.ProductId == Id);
                
            return View(objProduct);
        }

        [Route("{Alias}", Name = ("ProductCategory"))]
        public IActionResult ProductCategory(string Alias, int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 5;
                var Category = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
                var ListCategory = _context.Categories.ToList();
                var ListBrand = _context.Brands.ToList();
                var ListProduct = _context.Products
                    .AsNoTracking()
                    .Where(n => n.CategoryId == Category.Id)
                    .OrderByDescending(n => n.Id);
                var count = 0;//dùng để đếm tổng số sản phẩm
                foreach (var item in ListProduct)
                {
                    count += 1;
                }
                ViewBag.Count = count;
                PagedList<Product> models = new PagedList<Product>(ListProduct, pageNumber, pageSize);
                ViewBag.CurrentCategory = Category;
                ViewBag.ListCategory = ListCategory;
                ViewBag.ListBrand = ListBrand;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("thuonghieu/{Alias}", Name = ("ProductBrand"))]
        public IActionResult ProductBrand(string Alias, int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 5;
                var ListCategory = _context.Categories.ToList();
                var Brand = _context.Brands.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
                var ListBrand = _context.Brands.ToList();
                var ListProduct = _context.Products
                    .AsNoTracking()
                    .Where(n => n.BrandId == Brand.Id)
                    .OrderByDescending(n => n.Id);
                var count = 0;//dùng để đếm tổng số sản phẩm
                foreach (var item in ListProduct)
                {
                    count += 1;
                }
                ViewBag.Count = count;
                PagedList<Product> models = new PagedList<Product>(ListProduct, pageNumber, pageSize);
                ViewBag.CurrentBrand = Brand;
                ViewBag.ListCategory = ListCategory;
                ViewBag.ListBrand = ListBrand;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
