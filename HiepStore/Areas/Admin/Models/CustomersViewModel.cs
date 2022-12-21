using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace HiepStore.Areas.Admin.Models
{
    public class CustomersViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ")]
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? LocationId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(8, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 8 ký tự")]
        public string? Password { get; set; }

        [MinLength(8, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 8 ký tự")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Nhập lại mật khẩu không đúng")]
        public string? ConfirmPassword { get; set; }

        public string? Salt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}
