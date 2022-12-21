using System.ComponentModel.DataAnnotations;

namespace HiepStore.Areas.Admin.Models
{
    public class NewsViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = ("Vui lòng nhập tiêu đề bài viết"))]
        public string? Title { get; set; }
        public string? Contents { get; set; }
        public string? Thumb { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public string? Alias { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Author { get; set; }
        public int? AccountId { get; set; }
        [Required(ErrorMessage = ("Vui lòng Chọn thẻ bài viết"))]
        public int? TagsId { get; set; }
        public int? CategoryId { get; set; }
        public bool IsHot { get; set; }
        public bool IsNewfeed { get; set; }
        public string? MetaKey { get; set; }
        public string? MetaDesc { get; set; }
        public int? Views { get; set; }
        public int? BrandId { get; set; }
        public int? ProductId { get; set; }
        public string? Subtitle { get; set; }

    }
}
