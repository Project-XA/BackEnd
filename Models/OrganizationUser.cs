using Models;
using Project_X.Models.Enums;

namespace Project_X.Models
{
    public class OrganizationUser
    {
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public UserRole Role { get; set; }
    }
}
