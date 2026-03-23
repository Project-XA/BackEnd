using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class AddStudentToSectionDTO
    {
        [Required(ErrorMessage = "Section ID is required")]
        public int SectionId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }
    }
}
