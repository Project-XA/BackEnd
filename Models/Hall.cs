using Models;

namespace Project_X.Models
{
    public class Hall
    {
        public int Id { get; set; }
        public string HallName { get; set; }
        public int Capacity { get; set; } // Maximum number of people the hall can accommodate
        public double HallArea { get; set; } // Area of the hall in square meters
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public List<AttendanceSession> Sessions { get; set; }
    }
}
