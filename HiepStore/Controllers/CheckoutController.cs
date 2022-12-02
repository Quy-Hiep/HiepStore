using AspNetCoreHero.ToastNotification.Abstractions;
using HiepStore.Extension;
using HiepStore.Helpper;
using HiepStore.Models;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace HiepStore.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }
        public CheckoutController(db_hiep_storeContext context, INotyfService notyfService)
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
        //[Authorize]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl)
        {
            var strSession = HttpContext.Session.GetString("CustomerId");
            //nếu Session đăng nhập bằng null thì gọi trang đăng nhập kèm url return 
            if (strSession == null)
            {
                string Urlreturn = "/checkout.html";
                return Redirect("/dang-nhap.html?returnUrl=" + System.Net.WebUtility.UrlEncode(Urlreturn));

            }

            //Lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.Id;
                model.FirstName  = khachhang.FirstName;
                model.LastName  = khachhang.LastName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
            }
            ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Location", "Name");
            ViewBag.GioHang = cart;
            return View(model);
        }

        [HttpPost]
        //[Authorize]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            //Lay ra gio hang de xu ly
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuaHangVM model = new MuaHangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(taikhoanID));

                //khachhang.LocationId = muaHang.TinhThanh;
                //khachhang.District = muaHang.QuanHuyen;
                //khachhang.Ward = muaHang.PhuongXa;
                khachhang.Address = muaHang.Address;
                _context.Update(khachhang);
                _context.SaveChanges();

                model.CustomerId = khachhang.Id;
                model.FirstName = khachhang.FirstName;
                model.LastName = khachhang.LastName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Address;
                model.Note = muaHang.Note;
            }
            try
            {
                //if (ModelState.IsValid)
                //{
                //Khoi tao don hang
                Order donhang = new Order();
                donhang.CustomerId = model.CustomerId;
                donhang.Address = model.Address;
                //donhang.LocationId = model.TinhThanh;
                //donhang.District = model.QuanHuyen;
                //donhang.Ward = model.PhuongXa;

                donhang.OrderDate = DateTime.Now;
                donhang.TransactStatusId = 1;//Don hang moi
                donhang.IsDeleted = false;
                donhang.IsPaid = false;
                donhang.Note = Utilities.StripHTML(model.Note);
                donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalPriceDiscount))-15000;//15k phí vận chuyển
                _context.Add(donhang);
                _context.SaveChanges();
                //tao danh sach don hang

                foreach (var item in cart)
                {
                    OrderDetail orderDetail = new OrderDetail();
                    orderDetail.OrderId = donhang.Id;
                    orderDetail.ProductId = item.product.Id;
                    orderDetail.Amount = item.amount;
                    orderDetail.TotalMoney = donhang.TotalMoney;
                    orderDetail.Price = item.product.Price;
                    orderDetail.CreatedAt = DateTime.Now;
                    _context.Add(orderDetail);
                }
                _context.SaveChanges();
                //clear gio hang
                HttpContext.Session.Remove("GioHang");
                //Xuat thong bao
                _notyfService.Success("Đơn hàng đặt thành công");
                try
                {
                    MailMessage mail = new MailMessage();
                    // you need to enter your mail address
                    mail.From = new MailAddress("quyhiep653@gmail.com");

                    //To Email Address - your need to enter your to email address
                    mail.To.Add(donhang.Customer.Email);

                    mail.Subject = "Thông tin đơn hàng";

                    //you can specify also CC and BCC - i will skip this
                    //mail.CC.Add("");
                    //mail.Bcc.Add("");

                    mail.IsBodyHtml = true;

                    string content = "<br/>HIEPSTORE<br/>Tuyệt vời.Đơn hàng của bạn đang trên đường đến. Bên dưới là thông tin chi tiết đơn hàng của bạn." +
                        "<br/>Theo dõi đơn hàng của bạn [link]" +
                        "<br/>TÓM TẮT ĐƠN HÀNG:" +
                        "<br/>Mã đơn hàng #: " + donhang.Id +
                        "<br/>Ngày đăt hàng: " + donhang.OrderDate +
                        "<br/>Tổng tiền: " + donhang.TotalMoney +
                        "<br/>ĐỊA CHỈ GIAO HÀNG: " + donhang.Address;

                    mail.Body = content;


                    //create SMTP instant

                    //you need to pass mail server address and you can also specify the port number if you required
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                    //Create nerwork credential and you need to give from email address and password
                    NetworkCredential networkCredential = new NetworkCredential("quyhiep653@gmail.com", "rnknvrpgnfayssnw");
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = networkCredential;
                    smtpClient.Port = 587; // this is default port number - you can also change this
                    smtpClient.EnableSsl = true; // if ssl required you need to enable it
                    smtpClient.Send(mail);

                    ViewBag.Message = "Thông tin đơn hàng đã dc gửi tới email của quý khách";
                }
                catch (Exception ex)
                {
                    //If any error occured it will show
                    ViewBag.Message = ex.Message.ToString();
                }


                //cap nhat thong tin khach hang
                return RedirectToAction("Success");


                //}
            }
            catch
            {
                ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Location", "Name");
                ViewBag.GioHang = cart;
                return View(model);
            }
            ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Location", "Name");
            ViewBag.GioHang = cart;
            return View(model);
        }
        [Route("dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
                }
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(taikhoanID));
                var donhang = _context.Orders
                    .Where(x => x.CustomerId == Convert.ToInt32(taikhoanID))
                    .OrderByDescending(x => x.OrderDate)
                    .FirstOrDefault();
                MuaHangSuccessVM successVM = new MuaHangSuccessVM();
                successVM.FirtName = khachhang.FirstName;
                successVM.LastName = khachhang.LastName;
                successVM.DonHangID = donhang.Id;
                successVM.TotalMoney = donhang.TotalMoney;
                successVM.Phone = khachhang.Phone;
                successVM.Email = khachhang.Email;
                successVM.Address = khachhang.Address;
                //successVM.PhuongXa = GetNameLocation(donhang.Ward.Value);
                //successVM.TinhThanh = GetNameLocation(donhang.District.Value);
                return View(successVM);
            }
            catch
            {
                return View();
            }
        }
        public string GetNameLocation(int idlocation)
        {
            try
            {
                var location = _context.Locations.AsNoTracking().SingleOrDefault(x => x.Id == idlocation);
                if (location != null)
                {
                    return location.NameWithType;
                }
            }
            catch
            {
                return string.Empty;
            }
            return string.Empty;
        }
    }
}
