using Project_X.Models.Enums;

namespace Project_X.Models.DTOs
{
    public class AttendanceRecordDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime TimeStamp { get; set; }
        public VerificationType? VerificationType { get; set; }
        public double? MatchScore { get; set; }
    }
}
