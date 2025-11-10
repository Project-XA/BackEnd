using Models.Enums;
using Project_X.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; }
        public Status Status { get; set; }
        public List<AttendanceSession> Sessions { get; set; }
        public List<VerificationSession> VerificationSessions { get; set; }
        public List<AttendanceLog> Logs { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
