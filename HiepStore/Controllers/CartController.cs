﻿using AspNetCoreHero.ToastNotification.Abstractions;
using HiepStore.Extension;
using HiepStore.Models;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Mvc;
namespace HiepStore.Controllers
{
    public class CartController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }
        public CartController(db_hiep_storeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        public List<CartItem> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }

        [HttpPost]
        //[Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            List<CartItem> cart = GioHang;

            try
            {
                //Them san pham vao gio hang
                CartItem item = cart.SingleOrDefault(p => p.product.Id == productID);
                if (item != null) // da co => cap nhat so luong
                {
                    item.amount = item.amount + amount.Value;
                    //luu lai session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);

                }
                else
                {
                    Product hh = _context.Products.SingleOrDefault(p => p.Id == productID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = hh
                    };
                    cart.Add(item);//Them vao gio
                }

                //Luu lai Session
                HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                _notyfService.Success("Thêm Vào giỏ hàng thành công");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [HttpPost]
        //[Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            //Lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.Id == productID);
                    if (item != null && amount.HasValue) // da co -> cap nhat so luong
                    {
                        item.amount = amount.Value;
                    }
                    //Luu lai session
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                    _notyfService.Success("Cập nhật giỏ hàng thành công");
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        //[Route("api/cart/remove")]
        public ActionResult Remove(int productID)
        {

            try
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p => p.product.Id == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                //luu lai session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                _notyfService.Success("Xóa khỏi giỏ hàng thành công");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [Route("cart", Name = "Cart")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}
