using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Shipper
    {
        public int Id { get; set; }
        public string? Shippername { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public DateTime? ShipAt { get; set; }
    }
}
