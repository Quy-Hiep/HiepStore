using Microsoft.AspNetCore.Mvc;

namespace HiepStore.Areas.Admin.Controllers
{
    public class AccountsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
