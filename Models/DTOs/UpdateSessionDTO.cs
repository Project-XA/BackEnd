
using Models;
using Project_X.Models.CustomAttributes;
using Project_X.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class UpdateSessionDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]{3,50}$",
            ErrorMessage = "Session name must be between 3 and 50 characters and contain only letters, numbers, and spaces.")]
        public string SessionName { get; set; }
        
        [Required]
        public ConnectionType ConnectionType { get; set; }
        public decimal Longitude { get; set; }
        public decimal latitude { get; set; }
        [Required]
        public double allowedRadius { get; set; }
        [Required]
        public string NetworkSSID { get; set; }
        [Required]
        public string NetworkBSSID { get; set; } //Mac Address
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartAt { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [EndTimeAfterStart("StartAt")]
        public DateTime EndAt { get; set; }
        [Required]
        public int HallId { get; set; }
    }
}
