using System.ComponentModel.DataAnnotations;

namespace HiepStore.Areas.Admin.Models
{
    public class PageViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = ("Vui lòng nhập tên trang"))]
        public string? Name { get; set; }
        public string? Contents { get; set; }
        public string? Thumb { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public string? Title { get; set; }
        public string? MetaDesc { get; set; }
        public string? MetaKey { get; set; }
        public string? Alias { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? Ordering { get; set; }

    }
}
