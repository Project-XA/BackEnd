
namespace Project_X.Models.DTOs
{
    public class UserStatisticsDTO
    {
        public int TotalSessions { get; set; }
        public int AttendedSessions { get; set; }
        public int MissedSessions { get; set; }
        public double AttendancePercentage { get; set; }
    }
}
