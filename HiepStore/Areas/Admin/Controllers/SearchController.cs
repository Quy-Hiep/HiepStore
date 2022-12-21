using HiepStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HiepStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly db_hiep_storeContext _context;

        public SearchController(db_hiep_storeContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            ls = _context.Products.AsNoTracking()
                                  .Include(a => a.Category)
                                  .Where(x => x.Name.Contains(keyword))
                                  .OrderByDescending(x => x.Name)
                                  .Take(10)
                                  .ToList();
            if (ls == null)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("ListProductsSearchPartial", ls);
            }
        }
        [HttpPost]
        public IActionResult FindCustomer(string keyword)
        {
            List<Customer> ls = new List<Customer>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListCustomersSearchPartial", null);
            }
            ls = _context.Customers.AsNoTracking()
                .Where(x =>
                x.FirstName.Contains(keyword) ||
                x.LastName.Contains(keyword) ||
                x.Email.Contains(keyword) ||
                x.Phone.Contains(keyword) ||
                x.Address.Contains(keyword))
                .OrderByDescending(x => x.FirstName)
                .Take(10).ToList();

            if (ls == null)
            {
                return PartialView("ListCustomersSearchPartial", null);
            }
            else
            {
                return PartialView("ListCustomersSearchPartial", ls);
            }
        }
        public IActionResult FindAccount(string keyword)
        {
            List<Account> ls = new List<Account>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListAccountsSearchPartial", null);
            }
            ls = _context.Accounts.AsNoTracking()
                .Where(x =>
                    x.FirstName.Contains(keyword) ||
                    x.LastName.Contains(keyword) ||
                    x.Email.Contains(keyword) ||
                    x.Phone.Contains(keyword))
                .OrderByDescending(x => x.Id)
                .Take(10).ToList();

            if (ls == null)
            {
                return PartialView("ListAccountsSearchPartial", null);
            }
            else
            {
                return PartialView("ListAccountsSearchPartial", ls);
            }
        }
    }
}
