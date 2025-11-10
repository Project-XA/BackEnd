using Project_X.Models;

namespace Models
{
    public class Organization
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationType { get; set; }
        public string ConatactEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<AttendanceSession> Sessions { get; set; }
        public List<AppUser> Users { get; set; }
    }
}
