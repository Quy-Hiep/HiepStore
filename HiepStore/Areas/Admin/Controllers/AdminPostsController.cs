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
    public class AdminPostsController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }
        public AdminPostsController(db_hiep_storeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminTinDangs
        public IActionResult Index(int? page)
        {
            var collection = _context.Posts.AsNoTracking()
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
            var lsPosts = _context.Posts
                .Include(n => n.Tags)
                .AsNoTracking()
                .Where(n=>n.IsDeleted==false)
                .OrderBy(x => x.Id);
            PagedList<Post> models = new PagedList<Post>(lsPosts, pageNumber, pageSize);

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

            var post = await _context.Posts
                .Include(n => n.Tags)
                .Include(n => n.Category)
                .Include(n => n.Brand)
                .Include(n => n.Product)
                .Include(n => n.Account)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
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

        // POST: Admin/AdminTinDangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            try
            {
                //Xử lý thumb
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string imageName = Utilities.SEOUrl(post.Title) + extension;
                    post.Thumb = await Utilities.UploadFile(fThumb, @"posts", imageName.ToLower());
                }
                if (string.IsNullOrEmpty(post.Thumb)) post.Thumb = "default.jpg";

                var strAccountId = HttpContext.Session.GetString("AccountId");
                var taikhoan = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Id == Convert.ToInt32(strAccountId));
                post.Author = taikhoan.LastName + " " + taikhoan.FirstName;
                post.AccountId = taikhoan.Id;
                post.Alias = Utilities.SEOUrl(post.Title);
                post.CreatedAt = DateTime.Now;

                _context.Add(post);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm mới thành công");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View(post);
            }
        }

        // GET: Admin/AdminTinDangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewBag.listTag = new SelectList(_context.Tags, "Id", "Name");
            ViewBag.listCategory = new SelectList(_context.Categories, "Id", "Name");
            ViewBag.listBrand = new SelectList(_context.Brands, "Id", "Name");
            ViewBag.listProduct = new SelectList(_context.Products, "Id", "Name");
            return View(post);
        }

        // POST: Admin/AdminTinDangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Title, Contents, Thumb, IsPublished, IsDeleted, Alias, CreatedAt, Author, AccountId, TagsId, CategoryId, IsHot, IsNewfeed, MetaKey, MetaDesc, Views, BrandId, ProductId, Subtitle")] Post post, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            try
            {
                //Xu ly Thumb
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string imageName = Utilities.SEOUrl(post.Title) + extension;
                    post.Thumb = await Utilities.UploadFile(fThumb, @"posts", imageName.ToLower());
                }
                if (string.IsNullOrEmpty(post.Thumb)) post.Thumb = "default.jpg";
                post.Alias = Utilities.SEOUrl(post.Title);

                _context.Update(post);
                await _context.SaveChangesAsync();
                _notyfService.Success("Chỉnh sửa thành công");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TinDangExists(post.Id))
                {
                    return NotFound();
                }
                else
                {
                return View(post);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        //Index bài viết trong Thùng rác
        public ActionResult Trash(int page = 1)
        {
            var pageNumber = page;
            var pageSize = 10;

            List<Post> lsPosts = new List<Post>();
            lsPosts = _context.Posts
            .AsNoTracking()
            .Include(n=>n.Tags)
            .Where(n => n.IsDeleted == true)
            .OrderBy(x => x.Id).ToList();


            PagedList<Post> models = new PagedList<Post>(lsPosts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }

        //Đưa bài viết vào thùng rác
        public ActionResult DeleteTrash(int id)
        {
            Post post = _context.Posts.Find(id);
            post.IsDeleted = true;
            _context.Entry(post).State = EntityState.Modified;
            _context.SaveChanges();
            _notyfService.Success("Đưa vào thùng rác thành công");
            return RedirectToAction("Index");
        }

        //khôi phục bài viết từ thùng rác
        public ActionResult ReTrash(int Id)
        {
            Post post = _context.Posts.Find(Id);
            post.IsDeleted = false;
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

            var tinDang = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tinDang == null)
            {
                return NotFound();
            }

            return View(tinDang);
        }

        // POST: Admin/AdminTinDangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tinDang = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(tinDang);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool TinDangExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
