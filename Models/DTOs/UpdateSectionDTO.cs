using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class UpdateSectionDTO
    {
        [Required(ErrorMessage = "Section Name is required")]
        public string SectionName { get; set; }
    }
}
