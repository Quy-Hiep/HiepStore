using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Location
    {
        public Location()
        {
            Customers = new HashSet<Customer>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Parent { get; set; }
        public int? Levels { get; set; }
        public string? Slug { get; set; }
        public string? NameWithType { get; set; }
        public string? Type { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
