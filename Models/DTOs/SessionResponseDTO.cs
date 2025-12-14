
using Project_X.Models.Enums;

namespace Project_X.Models.DTOs
{
    public class SessionResponseDTO
    {
        public int SessionId { get; set; }
        public int OrganizationId { get; set; }
        public string CreatedBy { get; set; }
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
    }
}
