using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class GetUserDTO
    {
        [Required(ErrorMessage = "Organization code is required")]
        public int OrgainzatinCode { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage ="Invalid email format")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
