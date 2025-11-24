using Models;

namespace Project_X.Models
{
    public class OrganizationUser
    {
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string Role { get; set; }
    }
}
