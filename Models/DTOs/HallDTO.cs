using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Project_X.Models.DTOs
{
    public class HallDTO
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]{3,100}$",
            ErrorMessage = "Hall name must be between 3 and 100 characters and contain only letters, numbers, and spaces.")]
        public string HallName { get; set; }
        [Required]
        public int Capacity { get; set; } // Maximum number of people the hall can accommodate
        [Required]
        public double HallArea { get; set; } // Area of the hall in square meters
        [Required]
        public int OrganizationId { get; set; }
    }
}
