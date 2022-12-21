using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiepStore.Areas.Admin.Controllers.Components
{
    public class AdminHeaderViewComponent : ViewComponent
    {
        private readonly db_hiep_storeContext _context;

        public AdminHeaderViewComponent(db_hiep_storeContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var idAdmin = HttpContext.Session.GetString("AccountId");
            var admin = _context.Accounts.AsNoTracking()
                .Include(x => x.Role)
                .SingleOrDefault(x => x.Id == Convert.ToInt32(idAdmin));
            return View(admin);
        }
    }
}
