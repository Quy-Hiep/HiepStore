using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class ResetPass
    {
        public int Id { get; set; }
        public string? MEmail { get; set; }
        public string? MToken { get; set; }
        public DateTime? MTime { get; set; }
        public int? MNumcheck { get; set; }
    }
}
