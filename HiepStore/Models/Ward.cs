using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Ward
    {
        public Ward()
        {
            Customers = new HashSet<Customer>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}
