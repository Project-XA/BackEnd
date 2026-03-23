using Models;

namespace Project_X.Models
{
    public class SectionUser
    {
        public int SectionId { get; set; }
        public Section Section { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
