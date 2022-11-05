using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Post
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
        public string? Tags { get; set; }
        public int? CategoryId { get; set; }
        public bool IsHot { get; set; }
        public bool IsNewfeed { get; set; }
        public string? MetaKey { get; set; }
        public string? MetaDesc { get; set; }
        public int? Views { get; set; }
    }
}
