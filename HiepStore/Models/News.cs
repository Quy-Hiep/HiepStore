using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class News
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Contents { get; set; }
        public string? Thumb { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public string? Alias { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Author { get; set; }
        public int? AccountId { get; set; }
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

        public virtual Account? Account { get; set; }
        public virtual Brand? Brand { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Tag? Tags { get; set; }
    }
}
