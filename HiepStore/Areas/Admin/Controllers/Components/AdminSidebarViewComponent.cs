using HiepStore.Models;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiepStore.Areas.Admin.Controllers.Components
{
    public class AdminSidebarViewComponent : ViewComponent
    {

        private readonly db_hiep_storeContext _context;

        public AdminSidebarViewComponent(db_hiep_storeContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
