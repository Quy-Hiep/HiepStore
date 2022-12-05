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
using Microsoft.AspNetCore.Authentication.Google;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [HttpGet]
        public IActionResult Dashboard()
        {
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(taikhoanID));
                var recentOrders = _context.Products.AsNoTracking().Take(4);//lấy sản phẩm vừa đặt gần đây(tạm thời lấy đại 4 cái sản phẩm trong kho)
                var favoriteProduct = _context.Products.AsNoTracking().Take(12);//Sản phẩm yêu thích
                
                if (khachhang != null)
                {
                    var lsDonHang = _context.Orders
                        .Include(x => x.TransactStatus)
                        .Include(x => x.Customer).AsNoTracking()
                        .Where(x => x.CustomerId == khachhang.Id)
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    ViewBag.DonHang = lsDonHang;
					ViewBag.recentOrders = recentOrders;
					ViewBag.favoriteProduct = favoriteProduct;
                    return View(khachhang);
                }

            }
            return RedirectToAction("Login");
        }


        [HttpPost]
		public async Task<IActionResult> EditCustomer(int id,  Customer cus, Microsoft.AspNetCore.Http.IFormFile fThumb)
		{
			if (id != cus.Id)
			{
				return NotFound();
			}

			try
			{
				var customer = await _context.Customers.FindAsync(id);
				cus.FirstName = Utilities.ToTitleCase(cus.FirstName);
				if (fThumb != null)
				{
					string extension = Path.GetExtension(fThumb.FileName);
					string image = Utilities.SEOUrl(cus.FirstName) + extension;
					customer.Avatar = await Utilities.UploadFile(fThumb, @"avatars", image.ToLower());
				}

				if (string.IsNullOrEmpty(customer.Avatar)) customer.Avatar = "default.jpg";
				customer.FirstName = cus.FirstName;
				customer.LastName = cus.LastName;
				customer.Email = cus.Email;
				customer.Phone = cus.Phone;
				customer.UpdatedAt = DateTime.Now;

				_context.Update(customer);
				_notyfService.Success("Cập nhật thành công");
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CustomerExists(cus.Id))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			return RedirectToAction("Dashboard", "Accounts");
		}

		private bool CustomerExists(int id)
		{
			return _context.Customers.Any(e => e.Id == id);
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
					_notyfService.Error("Tài khoản đã bị tạm khóa, vui lòng liên quản trị viên để mở lại");
					return View(customer);
                }

                //Luu Session CustomerId
                HttpContext.Session.SetString("CustomerId", khachhang.Id.ToString());
				khachhang.LastLogin= DateTime.Now;
				_context.Update(khachhang);
				await _context.SaveChangesAsync();
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
                //nếu rồi thì đăng nhập,
                //nếu chưa thì tiến hàng đăng kí(insert thông tin tài khoản fb vào table customers)
                if (khachhang != null)
                {
                    //lưu Session mã khách hàng
                    HttpContext.Session.SetString("CustomerId", khachhang.Id.ToString());
                    khachhang.LastLogin=DateTime.Now;
					_context.Update(khachhang);
					await _context.SaveChangesAsync();
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
                                Email = user.email.Trim().ToLower(),
                                LocationId = 1,//vì không lấy dc địa chỉ nên gán tạm địa chỉ, customer có thể tự sửa, hoặc admin có thể sửa lại
                                IsActive = true,
                                CreatedAt = DateTime.Now,
                                LastLogin= DateTime.Now
                            };
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


        public async Task LoginWithGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse", "Accounts")
            });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities
                .FirstOrDefault().Claims.Select(claim => new
                {
                    claim.Type,
                    claim.Value
                });
            //var strJson = Json(claims);
            var firstName = claims
                .Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")
                .Select(x => x.Value)
                .FirstOrDefault();

            var lastName = claims
                .Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")
                .Select(x => x.Value)
                .FirstOrDefault();
            
            var email = claims
                .Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                .Select(x => x.Value)
                .FirstOrDefault();

            var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == email);
            //kiểm tra xem email lấy từ tk google đã tồn tại trong database chưa.
            //nếu rồi thì đăng nhập,
            //nếu chưa thì tiến hàng đăng kí(insert thông tin tài khoản google vào table customers)
            if (khachhang != null)
            {
                //lưu Session mã khách hàng
                HttpContext.Session.SetString("CustomerId", khachhang.Id.ToString());
				khachhang.LastLogin = DateTime.Now;
				_context.Update(khachhang);
				await _context.SaveChangesAsync();
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
                            FirstName = firstName,
                            LastName = lastName,
                            //Phone = user.Phone.Trim().ToLower(),
                            Email = email.ToLower(),
                            IsActive = true,
                            CreatedAt = DateTime.Now,
                            LastLogin= DateTime.Now
                        };
                            _context.Add(customer);
                            await _context.SaveChangesAsync();
                            //Lưu Session CustomerId
                            HttpContext.Session.SetString("CustomerId", customer.Id.ToString());

                            _notyfService.Success("Đăng nhập với facebook thành công");
                            return RedirectToAction("Dashboard", "Accounts");
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
