using System;
using System.Collections.Generic;
using HiepStore.Models;

namespace HiepStore.ModelViews
{
    public class XemDonHang
    {
        public Order DonHang { get; set; }
        public List<OrderDetail> ChiTietDonHang { get; set; }
    }
}
