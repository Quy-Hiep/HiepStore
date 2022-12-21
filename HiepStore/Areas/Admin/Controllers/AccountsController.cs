using AspNetCoreHero.ToastNotification.Abstractions;
using HiepStore.Models;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HiepStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }
        public AccountController(db_hiep_storeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
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


        [AllowAnonymous]
        [Route("admin-login.html", Name = "Login")]
        public IActionResult AdminLogin(string returnUrl)
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID != null) return RedirectToAction("Index", "Home", new { Area = "Admin" });
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("admin-login.html", Name = "Login")]
        public async Task<IActionResult> AdminLogin(HiepStore.Areas.Admin.Models.LoginViewModel model, string returnUrl)
        {
            try
            {
                    Account kh = _context.Accounts
                    .Include(p => p.Role)
                    .SingleOrDefault(p => p.Email.ToLower() == model.UserName.ToLower().Trim());

                    if (kh == null)
                    {
                        ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
                    }
                    string pass = (model.Password.Trim());
                    if (kh.Password.Trim() != pass)
                    {
                        ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
                        return View(model);
                    }
                    //đăng nhập thành công

                    //ghi nhận thời gian đăng nhập
                    kh.LastLogin = DateTime.Now;
                    _context.Update(kh);
                    await _context.SaveChangesAsync();
                    //luuw seccion Makh
                    HttpContext.Session.SetString("AccountId", kh.Id.ToString());

                //identity
                //var userClaims = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Email, kh.Email),
                //    new Claim("AccountId", kh.Id.ToString()),
                //    //new Claim("RoleId", kh.RoleId.ToString()),
                //    //new Claim(ClaimTypes.Role, kh.Role.Name)
                //};
                //ClaimsIdentity grandmaIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                //ClaimsPrincipal userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                //await HttpContext.SignInAsync(userPrincipal);
            }
            catch
            {
                return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
            }
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("index", "Home", new { Area = "Admin" });
            }
            else
            {
                return Redirect(returnUrl);
            }
        }
        [Route("logout.html", Name = "Logout")]
        public IActionResult AdminLogout()
        {
            try
            {
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId");
                return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
            }
            catch
            {
                return RedirectToAction("AdminLogin", "Account", new { Area = "Admin" });
            }
        }

    }
}
