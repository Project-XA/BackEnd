using Models;
using Project_X.Models.Enums;

namespace Project_X.Models
{
    public class VerificationSession
    {
        public int VerificationId { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public int SessionId { get; set; }
        public AttendanceSession Session { get; set; }
        public AttendanceLog Log { get; set; }
        public DateTime TimeStamp { get; set; }
        public string NetworkSSID { get; set; }
        public VerificationType VerificationType { get; set; }
        public double MatchScore { get; set; }
        public bool IsSuccessed { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public byte[] ProofSignture { get; set; }

    }
}