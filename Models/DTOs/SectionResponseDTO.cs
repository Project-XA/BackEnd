namespace Project_X.Models.DTOs
{
    public class SectionResponseDTO
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public int SectionCode { get; set; }
        public int OrganizationId { get; set; }
        public int MemberCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
