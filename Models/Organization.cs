using Project_X.Models;
using System;
using System.Collections.Generic;

namespace Models
{
    public class Organization
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationType { get; set; }
        public string ConatactEmail { get; set; }
        public int OrganizationCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<AttendanceSession> Sessions { get; set; }
        public List<OrganizationUser> OrganizationUsers { get; set; }
        public List<Hall> Halls { get; set; }
    }
}
