using Project_X.Models.Enums;

namespace Project_X.Models
{
    public class LocationBeacon
    {
        public int BeaconId { get; set; }
        public int SessionId { get; set; }  
        public AttendanceSession Session { get; set; }
        public ConnectionType BeaconType { get; set; }
        public double SignalStrength { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
