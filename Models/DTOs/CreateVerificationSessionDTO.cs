using System.ComponentModel.DataAnnotations;
using Project_X.Models.Enums;

namespace Project_X.Models.DTOs
{
    public class CreateVerificationSessionDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int SessionId { get; set; }
        [Required]
        public VerificationType VerificationType { get; set; }
        public double MatchScore { get; set; }
        public bool IsSuccessed { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public byte[] ProofSignature { get; set; }
        public string NetworkSSID { get; set; }
    }
}
