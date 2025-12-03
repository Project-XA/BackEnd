using Project_X.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full Name can only contain letters and spaces")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "UserName can only contain letters, numbers, and underscores")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Confirm Email is required")]
        [DataType(DataType.EmailAddress)]
        [Compare("Email", ErrorMessage = "Email and Confirm Email do not match")]
        public string ConfirmEmail { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Your Role is required")]
        public UserRole Role { get; set; }
    }
}
