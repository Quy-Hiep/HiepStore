using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class District
    {
        public District()
        {
            Customers = new HashSet<Customer>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
