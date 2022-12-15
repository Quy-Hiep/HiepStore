using AspNetCoreHero.ToastNotification.Abstractions;
using HiepStore.Helpper;
using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using PagedList.Core;

namespace HiepStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminNewsController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }
        public AdminNewsController(db_hiep_storeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminTinDangs
        public IActionResult Index(int? page)
        {
            var collection = _context.News.AsNoTracking()
                .ToList();
            foreach (var item in collection)
            {
                if (item.CreatedAt == null)
                {
                    item.CreatedAt = DateTime.Now;
                    _context.Update(item);
                    _context.SaveChanges();
                }
            }

            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;
            var lsnewss = _context.News
                .Include(n => n.Tags)
                .AsNoTracking()
                .Where(n=>n.IsDeleted==false)
                .OrderBy(x => x.Id);
            PagedList<News> models = new PagedList<News>(lsnewss, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminTinDangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Tags)
                .Include(n => n.Category)
                .Include(n => n.Brand)
                .Include(n => n.Product)
                .Include(n => n.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: Admin/AdminTinDangs/Create
        public IActionResult Create()
        {
            List<Tag> listTag = _context.Tags.ToList();
            List<Category> listCategory = _context.Categories.ToList();
            List<Brand> listBrand = _context.Brands.ToList();
            List<Product> listProduct = _context.Products.OrderByDescending(n=>n.Id).ToList();
            SelectList selectListItemsTag = new SelectList(listTag, "Id", "Name");
            SelectList selectListItemsCategory = new SelectList(listCategory, "Id", "Name");
            SelectList selectListItemsBrand = new SelectList(listBrand, "Id", "Name");
            SelectList selectListItemsProduct = new SelectList(listProduct, "Id", "Name");
            ViewBag.listTag = selectListItemsTag;
            ViewBag.listCategory = selectListItemsCategory;
            ViewBag.listBrand = selectListItemsBrand;
            ViewBag.listProduct = selectListItemsProduct;

            return View();
        }

        // news: Admin/AdminTinDangs/Create
        // To protect from overnewsing attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            try
            {
                //Xử lý thumb
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string imageName = Utilities.SEOUrl(news.Title) + extension;
                    news.Thumb = await Utilities.UploadFile(fThumb, @"news", imageName.ToLower());
                }
                if (string.IsNullOrEmpty(news.Thumb)) news.Thumb = "default.jpg";

                var strAccountId = HttpContext.Session.GetString("AccountId");
                var taikhoan = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(strAccountId));
                news.Author = taikhoan.LastName + " " + taikhoan.FirstName;
                news.AccountId = taikhoan.Id;
                news.Alias = Utilities.SEOUrl(news.Title);
                news.CreatedAt = DateTime.Now;

                _context.Add(news);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm mới thành công");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View(news);
            }
        }

        // GET: Admin/AdminTinDangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            ViewBag.listTag = new SelectList(_context.Tags, "Id", "Name");
            ViewBag.listCategory = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.listBrand = new SelectList(_context.Brands, "Id", "Name");
            ViewBag.listProduct = new SelectList(_context.Products, "Id", "Name");
            return View(news);
        }

        // news: Admin/AdminTinDangs/Edit/5
        // To protect from overnewsing attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Title, Contents, Thumb, IsPublished, IsDeleted, Alias, CreatedAt, Author, AccountId, TagsId, CategoryId, IsHot, IsNewfeed, MetaKey, MetaDesc, Views, BrandId, ProductId, Subtitle")] News news, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != news.Id)
            {
                return NotFound();
            }
            try
            {
                //Xu ly Thumb
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string imageName = Utilities.SEOUrl(news.Title) + extension;
                    news.Thumb = await Utilities.UploadFile(fThumb, @"news", imageName.ToLower());
                }
                if (string.IsNullOrEmpty(news.Thumb)) news.Thumb = "default.jpg";
                news.Alias = Utilities.SEOUrl(news.Title);

                _context.Update(news);
                await _context.SaveChangesAsync();
                _notyfService.Success("Chỉnh sửa thành công");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TinDangExists(news.Id))
                {
                    return NotFound();
                }
                else
                {
                return View(news);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        //Index bài viết trong Thùng rác
        public ActionResult Trash(int page = 1)
        {
            var pageNumber = page;
            var pageSize = 10;

            List<News> lsnews = new List<News>();
            lsnews = _context.News
            .AsNoTracking()
            .Include(n=>n.Tags)
            .Where(n => n.IsDeleted == true)
            .OrderBy(x => x.Id).ToList();


            PagedList<News> models = new PagedList<News>(lsnews.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }

        //Đưa bài viết vào thùng rác
        public ActionResult DeleteTrash(int id)
        {
            News news = _context.News.Find(id);
            news.IsDeleted = true;
            _context.Entry(news).State = EntityState.Modified;
            _context.SaveChanges();
            _notyfService.Success("Đưa vào thùng rác thành công");
            return RedirectToAction("Index");
        }

        //khôi phục bài viết từ thùng rác
        public ActionResult ReTrash(int Id)
        {
            News news = _context.News.Find(Id);
            news.IsDeleted = false;
            _context.SaveChanges();
            _notyfService.Success("Khôi phục thành công");
            return RedirectToAction("Trash");
        }

        // GET: Admin/AdminTinDangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tinDang = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tinDang == null)
            {
                return NotFound();
            }

            return View(tinDang);
        }

        // news: Admin/AdminTinDangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tinDang = await _context.News.FindAsync(id);
            _context.News.Remove(tinDang);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool TinDangExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
