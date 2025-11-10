using Models;
using Project_X.Models.Enums;

namespace Project_X.Models
{
    public class AttendanceLog
    {
        public int LogId { get; set; }
        public int SessionId { get; set; }
        public string UserId { get; set; }
        public int VerificationId { get; set; }
        public AttendanceSession Session { get; set; }
        public AppUser User { get; set; }
        public VerificationSession VerificationSession { get; set; }
        public DateTime TimeStamp { get; set; }
        public AttendanceResult Result { get; set; }
        public byte[] ProofSignature { get; set; }
    }
}