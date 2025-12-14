
using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class UpdateOrganizationDTO
    {
        [Required(ErrorMessage ="Organization Name is required")]
        public string OrganizationName { get; set; }
        [Required(ErrorMessage = "Organization type is required")]
        public string OrganizationType { get; set; }
        [Required(ErrorMessage = "Conatct Email is required")]
        public string ConatactEmail { get; set; }
    }
}
