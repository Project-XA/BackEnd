using System;
using System.Collections.Generic;

namespace Project_X.Models.DTOs
{
    public class OrganizationResponseDTO
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationType { get; set; }
        public string ConatactEmail { get; set; }
        public int OrganizationCode { get; set; }
        public DateTime CreatedAt { get; set; }
        
    }
}
