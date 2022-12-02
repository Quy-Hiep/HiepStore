using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace HiepStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly db_hiep_storeContext _context;
        public CategoryController(ILogger<CategoryController> logger, db_hiep_storeContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("/danhmuc")]
        public IActionResult Index()
        {
            var ListCategory = _context.Categories.ToList();
            return View(ListCategory);
        }

    }
}
