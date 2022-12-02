using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiepStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin.html", Name = "AdminIndex")]
    public class HomeController : Controller
    {
        //[Authorize]
        public IActionResult Index()
        {
            var strSession = HttpContext.Session.GetString("AccountId");
            //nếu Session đăng nhập bằng null thì gọi trang đăng nhập kèm return url
            if (strSession == null)
            {
                string returnUrl = "/admin.html";
                return Redirect("/admin-login.html?returnUrl="+ System.Net.WebUtility.UrlEncode(returnUrl));

            }
            return View();
        }
    }
}
