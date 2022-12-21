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
using System.Net;
using System.IO;
using NuGet.Packaging.Signing;
using Org.BouncyCastle.Asn1.Cms;
using System.Security.Cryptography;
using System.Net.Mail;

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
                    //var cus = await _context.Customers.FindAsync(taikhoan.Email);
                    var cusEmail = _context.Customers.SingleOrDefault(x => x.Email == taikhoan.Email);
                    var cusPhone = _context.Customers.SingleOrDefault(x => x.Phone == taikhoan.Phone);
                    if (cusEmail != null)
                    {
                        _notyfService.Error("Email đã tồn tại trong hệ thống, vui lòng nhập email khác", 5);
						return View(taikhoan);
					}
					if (cusPhone != null)
					{
						_notyfService.Error("Số điện thoại đã tồn tại trong hệ thống, vui lòng nhập số điện thoại khác khác", 5);
						return View(taikhoan);
					}

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
                        CreatedAt = DateTime.Now,
                        LastLogin= DateTime.Now
                    };
                    try
                    {
                        _context.Add(khachhang);
                        await _context.SaveChangesAsync();
                        //Lưu Session MaKh
                        HttpContext.Session.SetString("CustomerId", khachhang.Id.ToString());
                        var taikhoanID = HttpContext.Session.GetString("CustomerId");

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

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
                {
                    claim.Type,
                    claim.Value
                });
            //return Json(claims);
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

            var avatar = claims
                .Where(x => x.Type == "urn:google:picture")
                .Select(x => x.Value)
                .FirstOrDefault();

            var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == email);
            //kiểm tra xem email lấy từ tk google đã tồn tại trong database chưa.
            //nếu rồi thì đăng nhập,
            //nếu chưa thì tiến hành đăng kí(insert thông tin tài khoản google vào table customers)
            if (khachhang != null)
            {
                //lưu Session mã khách hàng
                HttpContext.Session.SetString("CustomerId", khachhang.Id.ToString());
				khachhang.LastLogin = DateTime.Now;
				_context.Update(khachhang);
				await _context.SaveChangesAsync();
				_notyfService.Success("Đăng nhập với google thành công");
                return RedirectToAction("Dashboard", "Accounts");
            }
            else
            {
                try
                {
                    if (ModelState.IsValid)
                    {
						//tiến hành đăng kí
                        Customer customer = new Customer();
                        customer.FirstName = firstName;
                        customer.LastName = lastName;
                        customer.Email = email.ToLower();
                        customer.IsActive = true;
                        customer.CreatedAt = DateTime.Now;
                        customer.LastLogin = DateTime.Now;

						string imageName = Utilities.SEOUrl(firstName) + ".png";
						//string strPath = "D:/workspace/ASP.NET/HiepStore/HiepStore/wwwroot/assets/images/avatars/" + imageName;
						string strPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", "avatars", imageName);
						WebClient wc = new WebClient();
						byte[] bytes = wc.DownloadData(avatar);
						MemoryStream ms = new MemoryStream(bytes);
						System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
						img.Save(strPath);
                        customer.Avatar = imageName;
						_context.Add(customer);
                        await _context.SaveChangesAsync();
                        //Lưu Session CustomerId
                        HttpContext.Session.SetString("CustomerId", customer.Id.ToString());

                        _notyfService.Success("Đăng kí với google thành công");
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

        [HttpGet]
        [Route("quen-mat-khau", Name = "QuenMatKhau")]
        public IActionResult ForgotPassword()
		{
			return View();
        }
        [HttpPost]
		[Route("quen-mat-khau", Name = "QuenMatKhau")]
		public async Task<IActionResult> ForgotPassword(String email)
        {
			var user = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email == email);
			//nếu user bằng null
			//thì thông báo "email này chưa được sử dụng cho tài khoản nào, vui lòng nhập lại email" và return view()
			//nếu user có tồn tại thì
			//Tạo một chuỗi ngẫu nhiên có độ dài tương đối, ví dụ chuỗi ngẫu nhiên 16 ký tự.
			//Thêm mới một record vào bảng "reset_pass", với thông tin các field như sau:
			//id: không cần truyền vào, cái này là khóa chính, và mình đã đặt thành "auto increment" trước đó.
			//m_email: là địa chỉ email nhận được ở bước 1.
			//m_time: timestamp ở thời điểm hiện tại
			//m_numcheck: không cần truyền vào, mặc định giá trị này sẽ bằng "0" khi mới thêm vào như định nghĩa của bảng phía trên.
			//m_token: mã hóa một chiều chuỗi ký tự ngẫu nhiên đã tạo tại bước 3.
			//Gửi đường link reset pass cho người dùng thông qua email nhận được ở bước 1. Đường link reset pass sẽ có dạng: 
			//<a href="https://yourdomain.com/resetPassword.php?email={email}&key={key}">Nhấn vào đây để tiến hành đặt lại mật khẩu</a>
			if (user == null)
            {
				_notyfService.Error("Email không liên kết với tài khoản nào. vui lòng nhập lại email!");
				return View();
			}
			else
            {
				string token = Utilities.GetRandomKeyNonSpecialCharacters(16);

                ResetPass resetPass = new ResetPass();
				resetPass.MEmail = email;
				resetPass.MTime = DateTime.Now;
				resetPass.MNumcheck = 0;
				resetPass.MToken = token.ToMD5();
				_context.Add(resetPass);
				await _context.SaveChangesAsync();

                //tiến hành gửi mail
				MailMessage mail = new MailMessage();
				mail.From = new MailAddress("quyhiep653@gmail.com");
				mail.To.Add(email);
				mail.Subject = "Thông tin đặt lại mật khẩu";
				//you can specify also CC and BCC - i will skip this
				//mail.CC.Add("");
				//mail.Bcc.Add("");
				mail.IsBodyHtml = true;
                string content = "<br/>HIEPSTORE<br/>" +
                    "<p>Bạn đang thực hiện quá trình đặt lại mật khẩu. " +
                    "Vui lòng nhấn vào link bên dưới để tiếp tục</p>" +
                    "Lưu ý: link chỉ có hiệu lực trong 60p" +
					"<a href=\"https://localhost:7217/ResetPassword?email="+ email + "&key=" + token + "\">Nhấn vào đây để tiến hành đặt lại mật khẩu</a>" +
                    "Nếu bạn không phải là người yêu cầu đặt lại mật khẩu, vui lòng bỏ qua và không nhấn vào link";
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

				return RedirectToAction("SuccessSendMail", "Accounts", email);
			}
        }

		[Route("gui-mail-dat-lai-mat-khau-thanh-cong")]
		public IActionResult SuccessSendMail(string email) 
        {
            ViewBag.email = email;
            return View(); 
        }

		[Route("/ResetPassword")]
		public IActionResult ResetPassword( string email, string key)
        {
            var reset_pass = _context.ResetPasses
                .AsNoTracking()
                .Where(n => n.MEmail == email)
                .FirstOrDefault();
            if (reset_pass == null) 
            {
                _notyfService.Error("email không tồn tại, vui lòng nhập lại email để nhận link mới");
                return RedirectToAction("ForgotPassword", "Accounts");
            }
            else if(key.ToMD5()!= reset_pass.MToken)
            {
				reset_pass.MNumcheck += 1;
                if (reset_pass.MNumcheck > 3)
                {
					_context.ResetPasses.Remove(reset_pass);
					_context.SaveChanges();
					_notyfService.Error("Khóa không hợp lệ, Xác thực thất bại quá 3 lần, vui lòng nhập email để nhận link mới");
					return RedirectToAction("ForgotPassword", "Accounts");
				}
				//thông báo lỗi "khóa không hợp lệ vui lòng thử lại link" và return
                _notyfService.Error("Khóa không hợp lệ, vui lòng nhấn lại link xác thực trong email!");
				_context.ResetPasses.Update(reset_pass);
				_context.SaveChanges();

				return RedirectToAction("Index", "Home");
			}
            else
            {
                TimeSpan time = (TimeSpan)(reset_pass.MTime - DateTime.Now);
                double mTime = time.TotalMinutes;
				if (mTime > 60)
				{
					_context.ResetPasses.Remove(reset_pass);
					_context.SaveChanges();
					_notyfService.Error("Link hết hạn, vui lòng nhập email để nhận link mới");
					return RedirectToAction("ForgotPassword", "Accounts");
				}
                return View();
			}
        }

        [HttpPost]
		[Route("/ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
			var reset_pass = _context.ResetPasses.AsNoTracking().Where(n => n.MEmail == model.Email).FirstOrDefault();
			var taikhoan = _context.Customers.Where(n => n.Email == model.Email).FirstOrDefault();
			string passnew = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
			taikhoan.Password = passnew;
            
			HttpContext.Session.SetString("CustomerId", taikhoan.Id.ToString());
			taikhoan.LastLogin = DateTime.Now;
			_context.Update(taikhoan);
            _context.ResetPasses.Remove(reset_pass);
			_context.SaveChanges();
			_notyfService.Success("Thiết lập mật khẩu mới thành công");
			return RedirectToAction("Dashboard", "Accounts");
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
