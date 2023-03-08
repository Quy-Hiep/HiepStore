using AspNetCoreHero.ToastNotification.Abstractions;
using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace HiepStore.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class AdminRolesController : Controller
    {
        private readonly db_hiep_storeContext _context;
        public INotyfService _notyfService { get; }
        public AdminRolesController(db_hiep_storeContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }


        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var lsRoles = _context.Roles
                .AsNoTracking()
                .OrderBy(x => x.Id);
            PagedList<Role> models = new PagedList<Role>(lsRoles, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            try
            {
                _context.Add(role);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm mới thành công");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(role);
            }

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Role role, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != role.Id)
            {
                return NotFound();
            }
            try
            {
                _context.Update(role);
                await _context.SaveChangesAsync();
                _notyfService.Success("Chỉnh sửa thành công");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(role);
            }            
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }
    }
}
