﻿using System;
using System.ComponentModel.DataAnnotations;

namespace HiepStore.ModelViews
{
    public class MuaHangVM
    {

        public int CustomerId { get; set; }

        //[Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        //public string FullName { get; set; }
        

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Vui lòng nhập họ")]
        public string LastName { get; set; }

        public string Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "vui lòng nhập Địa chỉ nhận hàng")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Tỉnh/Thành")]
        public int TinhThanh { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public int QuanHuyen { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn Phường/Xã")]
        public int PhuongXa { get; set; }
        public int PaymentID { get; set; }
        public string Note { get; set; }
    }
}
