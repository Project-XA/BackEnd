namespace Project_X.Models.DTOs
{
    public class HallResponseDTO
    {
        public int Id { get; set; }
        public string HallName { get; set; }
        public int Capacity { get; set; }
        public double HallArea { get; set; }
        public int OrganizationId { get; set; }
    }
}
