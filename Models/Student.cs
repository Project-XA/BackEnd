using System;

namespace Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string AppUserId { get; set; }
        public AppUser User { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string RollNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
