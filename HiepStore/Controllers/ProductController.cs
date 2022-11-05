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
        public IActionResult Detail(int Id)
        {
            var objProduct = _context.Products.Where(n => n.Id == Id).FirstOrDefault();
            return View(objProduct);
        }

        public IActionResult ProductCategory(int Id, int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 5;
                var ListCategory = _context.Categories.ToList();
                var ListBrand = _context.Brands.ToList();
                var ListProduct = _context.Products
                    .AsNoTracking()
                    .Where(n => n.CategoryId == Id)
                    .OrderByDescending(n => n.Id);
                var count = 0;//dùng để đếm tổng số sản phẩm
                foreach (var item in ListProduct)
                {
                    count += 1;
                }
                ViewBag.Count = count;
                PagedList<Product> models = new PagedList<Product>(ListProduct, pageNumber, pageSize);
                ViewBag.Category = ListCategory;
                ViewBag.Brand = ListBrand;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult ProductBrand(int Id, int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 5;
                var ListCategory = _context.Categories.ToList();
                var ListBrand = _context.Brands.ToList();
                var ListProduct = _context.Products
                    .AsNoTracking()
                    .Where(n => n.BrandId == Id)
                    .OrderByDescending(n => n.Id);
                var count = 0;//dùng để đếm tổng số sản phẩm
                foreach (var item in ListProduct)
                {
                    count += 1;
                }
                ViewBag.Count = count;
                PagedList<Product> models = new PagedList<Product>(ListProduct, pageNumber, pageSize);
                ViewBag.Category = ListCategory;
                ViewBag.Brand = ListBrand;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
