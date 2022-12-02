using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiepStore.Controllers
{
    public class PageController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public PageController(db_hiep_storeContext context)
        {
            _context = context;
        }
        // GET: /<controller>/

        [Route("/page/{Alias}", Name = "PageDetails")]
        public IActionResult Details(string Alias)
        {
            if (string.IsNullOrEmpty(Alias)) return RedirectToAction("Index", "Home");

            var page = _context.Pages.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
            if (page == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(page);
        }
    }
}
