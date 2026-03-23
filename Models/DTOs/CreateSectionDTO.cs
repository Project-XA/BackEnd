using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class CreateSectionDTO
    {
        [Required(ErrorMessage = "Organization ID is required")]
        public int OrganizationId { get; set; }

        [Required(ErrorMessage = "Section Name is required")]
        public string SectionName { get; set; }
    }
}
