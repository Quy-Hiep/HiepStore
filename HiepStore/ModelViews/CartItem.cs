using System;
using HiepStore.Models;

namespace HiepStore.ModelViews
{
    public class CartItem
    {
        public Product product { get; set; }
        public int amount { get; set; }
        public double TotalPrice => amount * product.Price.Value;
        public double TotalPriceDiscount => amount * product.Discount.Value;
        public double TotalDiscount => amount * (product.Price.Value - product.Discount.Value);
    }
}
