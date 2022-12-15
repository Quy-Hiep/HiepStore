using System;
using System.Collections.Generic;

namespace HiepStore.Models
{
    public partial class Tag
    {
        public Tag()
        {
            News = new HashSet<News>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Alias { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}
