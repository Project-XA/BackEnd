using Project_X.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class AttendanceLogItemDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public AttendanceResult Result { get; set; }
        
    }
}
