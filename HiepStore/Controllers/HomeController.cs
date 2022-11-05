using HiepStore.Models;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace HiepStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly db_hiep_storeContext _context;

        public HomeController(ILogger<HomeController> logger, db_hiep_storeContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeModel objHomeModel = new HomeModel();
            objHomeModel.ListProduct = _context.Products.AsNoTracking()
                .Where(x => x.IsActive == true && x.IsShowOnHomePage == true)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
            objHomeModel.ListCategory = _context.Categories
                .AsNoTracking()
                .Where(x => x.IsPublished == true)
                .OrderByDescending(x => x.Ordering)
                .ToList();
            return View(objHomeModel);
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}