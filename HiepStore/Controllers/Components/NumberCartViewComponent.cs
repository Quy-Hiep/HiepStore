using HiepStore.Extension;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Mvc;

namespace HiepStore.Controllers.Components
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            return View(cart);
        }
    }
}
