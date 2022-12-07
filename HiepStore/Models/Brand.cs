using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Posts = new HashSet<Post>();
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public int? Levels { get; set; }
        public bool IsShowOnHomePage { get; set; }
        public int? Ordering { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public string? Thumb { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedUser { get; set; }
        public int? UpdateUser { get; set; }
        public string? Title { get; set; }
        public string? Alias { get; set; }
        public string? MetaDesc { get; set; }
        public string? MetaKey { get; set; }
        public string? SchemaMarkup { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
