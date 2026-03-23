using Models;

namespace Project_X.Models
{
    public class Section
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public int SectionCode { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public List<SectionUser> SectionUsers { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
