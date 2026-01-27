using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class SaveAttendDTO
    {
        [Required(ErrorMessage = "Session ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Session ID must be greater than 0")]
        public int SessionId { get; set; }

        [Required(ErrorMessage = "Attendance Logs list is required")]
        [MinLength(1, ErrorMessage = "At least one attendance log must be provided")]
        public List<AttendanceLogItemDTO> AttendanceLogs { get; set; }
    }
}
