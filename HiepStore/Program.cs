using AspNetCoreHero.ToastNotification;
using HiepStore.Models;
using HiepStore.ModelViews;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
var connectString = configuration.GetConnectionString("HiepStoreConnectionString");
services.AddDbContext<db_hiep_storeContext>(options => options.UseSqlServer(connectString));

services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));
services.AddSession();
//services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(p =>
//    {
//        //p.Cookie.Name = "UserLoginCookie";
//        p.ExpireTimeSpan = TimeSpan.FromDays(1);
//        p.LoginPath = "/dang-nhap.html";
//        p.LogoutPath = "/logout.html";
//        p.AccessDeniedPath = "/not-found.html";
//    });
services.AddControllersWithViews().AddRazorRuntimeCompilation();
services.AddNotyf(config =>
{
    config.DurationInSeconds = 3;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});
//services.AddAuthentication()
//    .AddGoogle(googleOptions =>
//    {
//        // Đọc thông tin Authentication:Google từ appsettings.json
//        IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");

//        // Thiết lập ClientID và ClientSecret để truy cập API google
//        googleOptions.ClientId = googleAuthNSection["ClientId"];
//        googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
//        // Cấu hình Url callback lại từ Google (không thiết lập thì mặc định là /signin-google)
//        googleOptions.CallbackPath = "/signin-google";

//    });
    //.AddFacebook(facebookOptions =>
    //{
    //    // Đọc cấu hình
    //    IConfigurationSection facebookAuthNSectionn = configuration.GetSection("Authentication:Facebook");
    //    facebookOptions.AppId = facebookAuthNSection["AppId"];
    //    facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];
    //    // Thiết lập đường dẫn Facebook chuyển hướng đến
    //    //facebookOptions.CallbackPath = "/dang-nhap-facebook";
    //});

var appSettings = configuration.GetSection("AppSettings");
services.Configure<AppSettings>(appSettings);

services.AddDistributedMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();
app.UseSession();


app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
