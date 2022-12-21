using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Product
    {
        public Product()
        {
            News = new HashSet<News>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ShortDesc { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? Price { get; set; }
        public int? Discount { get; set; }
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

        public virtual Brand? Brand { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ProductType? Type { get; set; }
        public virtual ICollection<News> News { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
