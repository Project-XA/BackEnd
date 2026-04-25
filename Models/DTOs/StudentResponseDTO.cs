namespace Project_X.Models.DTOs
{
    public class StudentResponseDTO
    {
        public int StudentId { get; set; }
        public string AppUserId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RollNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
