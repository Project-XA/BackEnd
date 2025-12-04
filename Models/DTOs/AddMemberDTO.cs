using Project_X.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class AddMemberDTO
    {
        [Required]
        public int OrganizationId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full Name can only contain letters and spaces")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "UserName is required")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "UserName can only contain letters, numbers, and underscores")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        public UserRole Role { get; set; } = UserRole.User; 
    }
}
