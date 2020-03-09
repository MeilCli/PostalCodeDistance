namespace PostalCodeDistance.NETStandard.Entities
{
    public class Address
    {
        public string Prefecture { get; }

        public string Municipality { get; }

        public string TownArea { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        public Address(string prefecture, string municipality, string townArea, double latitude, double longitude)
            => (Prefecture, Municipality, TownArea, Latitude, Longitude) = (prefecture, municipality, townArea, latitude, longitude);

        public override string ToString()
        {
            return $"Prefecture: {Prefecture}, Municipality: {Municipality}, TownArea: {TownArea}, Latitude: {Latitude}, Longitude: {Longitude}";
        }
    }
}
