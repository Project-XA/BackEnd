using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class JoinSectionDTO
    {
        [Required(ErrorMessage = "Section Code is required")]
        public int SectionCode { get; set; }
    }
}
