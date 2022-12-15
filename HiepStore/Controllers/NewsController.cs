using HiepStore.Models;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace HiepStore.Controllers
{
	public class NewsController : Controller
	{
		private readonly db_hiep_storeContext _context;
		public NewsController(db_hiep_storeContext context)
		{
			_context = context;
		}
		// GET: /<controller>/
		[Route("tin-tuc.html", Name = ("news"))]
        public IActionResult Index(int? page)
        {
            var ListNews =_context.News.AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.Product)
                .Include(x => x.Account)
                .Where(x => x.IsDeleted == false && x.IsPublished == true)
                .OrderByDescending(x => x.CreatedAt);

            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 8;
            PagedList<News> models = new PagedList<News>(ListNews, pageNumber, pageSize);
            var ListTag = _context.Tags.ToList();
            var ListCategory = _context.Categories.ToList();
            var ListBrand = _context.Brands.ToList();
            ViewBag.ListTag = ListTag;
            ViewBag.ListCategory = ListCategory;
            ViewBag.ListBrand = ListBrand;

            return View(models);
        }
        
        [Route("/tin-tuc/{Alias}-{id}", Name = "TinChiTiet")]
		public IActionResult Details(int id)
		{
			var tindang = _context.News.AsNoTracking()
                .Include(x => x.Tags)
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.Product)
                .Include(x => x.Account)
                .SingleOrDefault(x => x.Id == id);
			if (tindang == null)
			{
				return RedirectToAction("Index");
			}
			var lsBaivietlienquan = _context.News
                .Include(x => x.Tags)
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.Product)
                .Include(x => x.Account)
                .AsNoTracking()
				.Where(x => x.IsPublished == true && x.Id != id)
				.Take(2)
				.OrderByDescending(x => x.CreatedAt).ToList();
			ViewBag.Baivietlienquan = lsBaivietlienquan;
            //các ViewBag dùng cho header và footer
            ViewBag.ListTag = _context.Tags.ToList();
            ViewBag.ListCategory = _context.Categories.ToList();
            ViewBag.ListBrand = _context.Brands.ToList();

            return View(tindang);
		}

        [Route("/tin-tuc/tags/{Alias}-{id}")]
        public IActionResult NewsTags(string Alias, int id, int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 5;
                var Tag = _context.Tags.AsNoTracking().SingleOrDefault(x => x.Id == id);
                var ListNews = _context.News
                    .Include(x => x.Tags)
                    .Include(x => x.Category)
                    .Include(x => x.Brand)
                    .Include(x => x.Product)
                    .Include(x => x.Account)
                    .AsNoTracking()
                    .Where(n => n.TagsId == Tag.Id)
                    .OrderByDescending(n => n.Id);
                var count = 0;//dùng để đếm tổng số bài viết
                foreach (var item in ListNews)
                {
                    count += 1;
                }
                ViewBag.Count = count;
                PagedList<News> models = new PagedList<News>(ListNews, pageNumber, pageSize);
                ViewBag.CurrentTag = Tag;
                //các ViewBag dùng cho header và footer
                ViewBag.ListTag = _context.Tags.ToList();
                ViewBag.ListCategory = _context.Categories.ToList();
                ViewBag.ListBrand = _context.Brands.ToList();
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("/tin-tuc/thuong-hieu/{Alias}-{id}")]
        public IActionResult NewsBrands(string Alias, int id, int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 5;
                var brand = _context.Brands.AsNoTracking().SingleOrDefault(x => x.Id == id);
                var ListNews = _context.News
                    .Include(x => x.Tags)
                    .Include(x => x.Category)
                    .Include(x => x.Brand)
                    .Include(x => x.Product)
                    .Include(x => x.Account)
                    .AsNoTracking()
                    .Where(n => n.BrandId == brand.Id)
                    .OrderByDescending(n => n.Id);
                var count = 0;//dùng để đếm tổng số bài viết
                foreach (var item in ListNews)
                {
                    count += 1;
                }
                ViewBag.Count = count;
                PagedList<News> models = new PagedList<News>(ListNews, pageNumber, pageSize);
                ViewBag.CurrentBrand = brand;
                //các ViewBag dùng cho header và footer
                ViewBag.ListTag = _context.Tags.ToList();
                ViewBag.ListCategory = _context.Categories.ToList();
                ViewBag.ListBrand = _context.Brands.ToList();
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("/tin-tuc/danh-muc-san-pham/{Alias}-{id}")]
        public IActionResult NewsCategory(string Alias, int id, int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 5;
                var Category = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Id == id);
                var ListNews = _context.News
                    .Include(x => x.Tags)
                    .Include(x => x.Category)
                    .Include(x => x.Brand)
                    .Include(x => x.Product)
                    .Include(x => x.Account)
                    .AsNoTracking()
                    .Where(n => n.CategoryId == Category.Id)
                    .OrderByDescending(n => n.Id);
                var count = 0;//dùng để đếm tổng số bài viết
                foreach (var item in ListNews)
                {
                    count += 1;
                }
                ViewBag.Count = count;
                PagedList<News> models = new PagedList<News>(ListNews, pageNumber, pageSize);
                ViewBag.CurrentCategory = Category;
                //các ViewBag dùng cho header và footer
                ViewBag.ListTag = _context.Tags.ToList();
                ViewBag.ListCategory = _context.Categories.ToList();
                ViewBag.ListBrand = _context.Brands.ToList();
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        [Route("/tin-tuc/profile/{id}")]
        public IActionResult NewsProfile(int id, int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 5;
                var Profile = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Id == id);
                var ListNews = _context.News
                    .Include(x => x.Tags)
                    .Include(x => x.Category)
                    .Include(x => x.Brand)
                    .Include(x => x.Product)
                    .Include(x => x.Account)
                    .AsNoTracking()
                    .Where(n => n.AccountId == Profile.Id && n.IsDeleted == false && n.IsPublished == true)
                    .OrderByDescending(n => n.Id);
                var count = 0;//dùng để đếm tổng số bài viết
                foreach (var item in ListNews)
                {
                    count += 1;
                }
                ViewBag.Count = count;
                PagedList<News> models = new PagedList<News>(ListNews, pageNumber, pageSize);
                ViewBag.CurrentProfile = Profile;
                //các ViewBag dùng cho header và footer
                ViewBag.ListTag = _context.Tags.ToList();
                ViewBag.ListCategory = _context.Categories.ToList();
                ViewBag.ListBrand = _context.Brands.ToList();

                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}

