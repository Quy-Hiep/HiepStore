using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ShipAt { get; set; }
        public int TransactStatusId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaymentAt { get; set; }
        public int TotalMoney { get; set; }
        public int? PaymentId { get; set; }
        public string? Note { get; set; }
        public string? Address { get; set; }
        public int? LocationId { get; set; }
        public int? District { get; set; }
        public int? Ward { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual TransactStatus TransactStatus { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
