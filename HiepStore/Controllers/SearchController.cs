using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace HiepStore.Controllers
{
    public class SearchController : Controller
    {
        private readonly db_hiep_storeContext _context;

        public SearchController(db_hiep_storeContext context)
        {
            _context = context;
        }
        public ActionResult Search(string currentFilter, string SearchString, int? page)
        {
            //List<Product> lstProduct = new List<Product>();

            var lstProduct = _context.Products.AsNoTracking();

            //var lstProduct = new List<Product>();


            var ListCategory = _context.Categories.ToList();
            var ListBrand = _context.Brands.ToList();
            var count = 0;//dùng để đếm tổng số sản phẩm
            if (SearchString != null)
            {
                page = 1;
            }
            else
            {
                SearchString = currentFilter;
            }
            if (!string.IsNullOrEmpty(SearchString))
            {
                //lấy danh sách sản phẩm theo từ khóa tìm kiếm
                lstProduct = _context.Products.Where(n => n.IsDeleted == false && n.Name.Contains(SearchString));
            }
            else
            {
                //lấy all sản phẩm trong bảng product
                lstProduct = _context.Products.Where(n => n.IsDeleted == false);
            }
            //vòng lặp đếm tổng số sản phẩm kiếm được
            foreach (var item in lstProduct)
            {
                count += 1;
            }
            ViewBag.Count = count;
            ViewBag.Category = ListCategory;
            ViewBag.Brand = ListBrand;
            //số lượng item cua 1 trang
            int pageSize = 10;
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            //sắp xếp theo id sản phẩm,sp mới đưa lên đầu
            lstProduct = lstProduct.OrderByDescending(n => n.Id);

            PagedList<Product> models = new PagedList<Product>(lstProduct, pageNumber, pageSize);
            return View(models);
        }
    }
}
