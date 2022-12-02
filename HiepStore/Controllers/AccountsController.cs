using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using HiepStore.Extension;
using HiepStore.Models;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HiepStore.Helpper;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Plugins;
using HiepStore.Facebook.Core.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HiepStore.Controllers
{
    public class AccountsController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }

        private readonly AppSettings _appSettings;


        public AccountsController(db_hiep_storeContext context, INotyfService notyfService, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _notyfService = notyfService;
            _appSettings = appSettings.Value;


        }
        [HttpGet]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (khachhang != null)
                    return Json(data: "Số điện thoại : " + Phone + "đã được sử dụng");

                return Json(data: true);

            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (khachhang != null)
                    return Json(data: "Email : " + Email + " đã được sử dụng");
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }

        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(taikhoanID));
                if (khachhang != null)
                {
                    var lsDonHang = _context.Orders
                        .Include(x => x.TransactStatus)
                        .AsNoTracking()
                        .Where(x => x.CustomerId == khachhang.Id)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag.DonHang = lsDonHang;
                    return View(khachhang);
                }

            }
            return RedirectToAction("Login");
        }


        [HttpGet]
        [Route("dang-ky.html", Name = "DangKy")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("dang-ky.html", Name = "DangKy")]
        public async Task<IActionResult> Register(RegisterViewModel taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomKey();
                    Customer khachhang = new Customer
                    {
                        FirstName = taikhoan.FirstName,
                        LastName = taikhoan.LastName,
                        Phone = taikhoan.Phone.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                        IsActive = true,
                        Salt = salt,
                        CreatedAt = DateTime.Now
                    };
                    try
                    {
                        _context.Add(khachhang);
                        await _context.SaveChangesAsync();
                        //Lưu Session MaKh
                        HttpContext.Session.SetString("CustomerId", khachhang.Id.ToString());
                        var taikhoanID = HttpContext.Session.GetString("CustomerId");

                        //Identity
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name,khachhang.FirstName),
                            new Claim("CustomerId", khachhang.Id.ToString())
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        _notyfService.Success("Đăng ký thành công");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    catch
                    {
                        return RedirectToAction("Register", "Accounts");
                    }
                }
                else
                {
                    return View(taikhoan);
                }
            }
            catch
            {
                return View(taikhoan);
            }
        }

        [Route("dang-nhap.html", Name = "DangNhap")]
        public IActionResult Login(string returnUrl)
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            //kiểm tra xem đã có session login chưa, nếu có rồi thì tức là đã đăng nhập rồi, trả về trang dashboard tài khoản
            if (taikhoanID != null)
            {
                return RedirectToAction("Dashboard", "Accounts");
            }
            LoginViewModel loginViewModel = new LoginViewModel()
            {
                FacebookAppId = _appSettings.FacebookAppId,
                FacebookRedirectUri = _appSettings.FacebookRedirectUri
            };
            ViewData["returnUrl"] = returnUrl;
            return View(loginViewModel);
        }
        [HttpPost]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public async Task<IActionResult> Login(LoginViewModel customer, string returnUrl)
        {
            try
            {
                bool isEmail = Utilities.IsValidEmail(customer.UserName);
                if (!isEmail) return View(customer);

                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == customer.UserName);
                if (khachhang == null)
                {
                    _notyfService.Error("Email chưa kết nối tài khoản nào, hãy tìm tài khoản của bạn và đăng nhập");
                    return View(customer);
                }
                string pass = (customer.Password + khachhang.Salt.Trim()).ToMD5();
                if (khachhang.Password != pass)
                {
                    _notyfService.Error("Thông tin đăng nhập chưa chính xác");
                    ViewData["returnUrl"] = returnUrl;
                    return View(customer);
                }
                //kiem tra xem account co bi disable hay khong

                if (khachhang.IsActive == false)
                {
                    return RedirectToAction("ThongBao", "Accounts");
                }

                //Luu Session CustomerId
                HttpContext.Session.SetString("CustomerId", khachhang.Id.ToString());

                //Identity
                //var claims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Email, khachhang.Email),
                //    new Claim("CustomerId", khachhang.Id.ToString())

                //};
                //var claimsIdentity = new ClaimsIdentity(claims, "User Identity");
                //var claimsPrincipal = new ClaimsPrincipal(new[] { claimsIdentity });
                //await HttpContext.SignInAsync(claimsPrincipal);


                _notyfService.Success("Đăng nhập thành công");
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Dashboard", "Accounts");
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }
            catch
            {
                ViewData["returnUrl"] = returnUrl;
                return View(customer);
            }
        }


        [Route("signin-facebook", Name = "signin-facebook")]
        public async Task<IActionResult> LoginWithFacebookAsync(string code)
        {
            try
            {
                Login.AccessToken accessToken = await Facebook.Core.Services.Login.GetAccessTokenAsync(_appSettings.FacebookAppId, _appSettings.FacebookAppSecret, _appSettings.FacebookRedirectUri, code);

                User user = await Facebook.Core.Services.Graph.GetUserAsync(accessToken.access_token);

                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == user.email);
                //kiểm tra xem email lấy từ tk fb đã tồn tại trong database chưa.
                //nếu rồi thì đăng lưu Session đăng nhập,
                //nếu chưa thì tiến hàng đăng kí(insert thông tin tài khoản fb vào table customers)
                if (khachhang != null)
                {
                    //lưu Session mã khách hàng
                    HttpContext.Session.SetString("CustomerId", khachhang.Id.ToString());
                    _notyfService.Success("Đăng nhập thành công");
                    return RedirectToAction("Dashboard", "Accounts");
                }
                else
                {
                    try
                    {
                        if (ModelState.IsValid)
                        {
                            Customer customer = new Customer
                            {
                                FirstName = user.first_name,
                                LastName = user.last_name,
                                //Phone = user.Phone.Trim().ToLower(),
                                Email = user.email.Trim().ToLower(),
                                //Password = (taikhoan.Password + salt.Trim()).ToMD5(),
                                IsActive = true,
                                CreatedAt = DateTime.Now
                            };
                            try
                            {
                                _context.Add(customer);
                                await _context.SaveChangesAsync();
                                //Lưu Session CustomerId
                                HttpContext.Session.SetString("CustomerId", customer.Id.ToString());

                                //Identity
                                //var claims = new List<Claim>
                                //{
                                //    new Claim(ClaimTypes.Name,customer.FirstName),
                                //    new Claim("CustomerId", customer.Id.ToString())
                                //};
                                //ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                                //ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                                //await HttpContext.SignInAsync(claimsPrincipal);
                                _notyfService.Success("Đăng nhập với facebook thành công");
                                return RedirectToAction("Dashboard", "Accounts");
                            }
                            catch
                            {
                                return RedirectToAction("login", "Accounts");
                            }
                        }
                        else
                        {
                            return RedirectToAction("login", "Accounts");
                        }
                    }
                    catch
                    {
                        return RedirectToAction("login", "Accounts");
                    }

                }
            }
            catch
            {

                return RedirectToAction("login", "Accounts");
            }


        }


        [HttpGet]
        [Route("dang-xuat.html", Name = "DangXuat")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (taikhoanID == null)
                {
                    return RedirectToAction("Login", "Accounts");
                }
                if (ModelState.IsValid)
                {
                    var taikhoan = _context.Customers.Find(Convert.ToInt32(taikhoanID));
                    if (taikhoan == null) return RedirectToAction("Login", "Accounts");
                    var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    {
                        string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                        taikhoan.Password = passnew;
                        _context.Update(taikhoan);
                        _context.SaveChanges();
                        _notyfService.Success("Đổi mật khẩu thành công");
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                }
            }
            catch
            {
                _notyfService.Success("Thay đổi mật khẩu không thành công");
                return RedirectToAction("Dashboard", "Accounts");
            }
            _notyfService.Success("Thay đổi mật khẩu không thành công");
            return RedirectToAction("Dashboard", "Accounts");
        }
    }
}
