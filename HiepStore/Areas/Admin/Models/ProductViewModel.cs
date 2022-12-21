using System.ComponentModel.DataAnnotations;

namespace HiepStore.Areas.Admin.Models
{
    public class ProductViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = ("Vui lòng nhập Tên sản phẩm"))]
        public string Name { get; set; } = null!;
        public string? ShortDesc { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = ("Vui lòng nhập tên danh mục"))]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = ("Vui lòng nhập Tên thương hiệu"))]
        public int? BrandId { get; set; }

        [Required(ErrorMessage = ("Vui lòng nhập giá gốc"))]
        public int? Price { get; set; }

        [Required(ErrorMessage = ("Vui lòng nhập giá khuyến mãi"))]
        public int? Discount { get; set; }

        [Required(ErrorMessage = ("Vui lòng nhập số lượng"))]
        public int? UnitsInStock { get; set; }
        public string? Thumb { get; set; }
        public string? Video { get; set; }
        public int? TypeId { get; set; }
        public bool IsShowOnHomePage { get; set; }
        public int? Ordering { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserCreated { get; set; }
        public int? UserUpdated { get; set; }
        public string? Tags { get; set; }
        public string? Title { get; set; }
        public string? Alias { get; set; }
        public string? MetaDesc { get; set; }
        public string? MetaKey { get; set; }
        public string? Configuration { get; set; }

    }
}
