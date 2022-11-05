using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Account
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public string? Salt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int? RoleId { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual Role? Role { get; set; }
    }
}
