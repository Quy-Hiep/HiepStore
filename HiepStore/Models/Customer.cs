using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? LocationId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Password { get; set; }
        public string? Salt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public virtual District? District { get; set; }
        public virtual Location? Location { get; set; }
        public virtual Ward? Ward { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
