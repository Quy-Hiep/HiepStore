using Microsoft.AspNetCore.Mvc;

namespace HiepStore.Controllers
{
    public class AjaxContentController : Controller
    {
        public IActionResult NumberCart()
        {
            return ViewComponent("NumberCart");
        }
    }
}
