using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Tag
    {
        public Tag()
        {
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
