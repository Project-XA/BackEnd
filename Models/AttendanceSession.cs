using Models;
using Project_X.Models.Enums;

namespace Project_X.Models
{
    public class AttendanceSession
    {
        public int SessionId { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public List<LocationBeacon> Beacons { get; set; }
        public List<VerificationSession> Verifications { get; set; }
        public List<AttendanceLog> Logs { get; set; }
        public string CreatedBy { get; set; } // userid for the user that will create the session
        public AppUser User { get; set; }
        public string SessionName { get; set; }
        public DateTime CreatedAt { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public decimal Longitude { get; set; }
        public decimal latitude { get; set; }
        public double allowedRadius { get; set; }
        public string NetworkSSID { get; set; } 
        public string NetworkBSSID { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int HallId { get; set; }
        public Hall Hall { get; set; }
    }
}
