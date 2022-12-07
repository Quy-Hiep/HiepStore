using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace HiepStore.Controllers
{
	public class BlogController : Controller
	{
		private readonly db_hiep_storeContext _context;
		public BlogController(db_hiep_storeContext context)
		{
			_context = context;
		}
		// GET: /<controller>/
		[Route("blogs.html", Name = ("Blog"))]
		public IActionResult Index(int? page)
		{
			var pageNumber = page == null || page <= 0 ? 1 : page.Value;
			var pageSize = 10;
			var lsPosts = _context.Posts
				.AsNoTracking()
				.OrderBy(x => x.Id);
			PagedList<Post> models = new PagedList<Post>(lsPosts, pageNumber, pageSize);

			ViewBag.CurrentPage = pageNumber;
			return View(models);
		}
		[Route("/tin-tuc/{Alias}-{id}.html", Name = "TinChiTiet")]
		public IActionResult Details(int id)
		{
			var tindang = _context.Posts.AsNoTracking().SingleOrDefault(x => x.Id == id);
			if (tindang == null)
			{
				return RedirectToAction("Index");
			}
			var lsBaivietlienquan = _context.Posts
				.AsNoTracking()
				.Where(x => x.IsPublished == true && x.Id != id)
				.Take(3)
				.OrderByDescending(x => x.CreatedAt).ToList();
			ViewBag.Baivietlienquan = lsBaivietlienquan;
			return View(tindang);
		}
	}
}

