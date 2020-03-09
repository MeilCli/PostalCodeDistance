using System.Runtime.Serialization;

namespace PostalCodeDistance.NETStandard.Entities
{
    public class Point
    {
#pragma warning disable CS8618
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        public Point() { }
#pragma warning restore CS8618

        public Point(Point point) : this(point.Code, point.Latitude, point.Longitude) { }

        public Point(string code, double latitude, double longitude)
            => (Code, Latitude, Longitude) = (code, latitude, longitude);

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Point? other = obj as Point;
            if (other is null)
            {
                return false;
            }

            if (Code != other.Code)
            {
                return false;
            }

            return Latitude == other.Latitude && Longitude == other.Longitude;
        }
    }
}
