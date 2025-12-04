using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Project_X.Models;
using Project_X.Models.Enums;
using System;
using System.Collections.Generic;
namespace Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public Status Status { get; set; } = Status.Active;
        public List<AttendanceSession> Sessions { get; set; }
        public List<VerificationSession> VerificationSessions { get; set; }
        public List<AttendanceLog> Logs { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrganizationUser> OrganizationUsers { get; set; }
        public UserRole Role { get; set; }
    }
}