using HiepStore.Models;

namespace HiepStore.Areas.Admin.Models
{
    public class DasboardViewModel
    {
        public List<Product> ListProduct { get; set; }
        public List<Order> ListOrder { get; set; }
    }
}
