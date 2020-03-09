using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Utf8Json;

namespace PostalCodeDistance.NETStandard.Entities
{
    public class Location : IEquatable<Location>
    {
#pragma warning disable CS8618
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "prefecture")]
        public string Prefecture { get; set; }

        [DataMember(Name = "municipality")]
        public string Municipality { get; set; }

        [DataMember(Name = "town_area")]
        public string? TownArea { get; set; }

        [DataMember(Name = "sub_town_areas")]
        public IReadOnlyList<string>? SubTownAreas { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        public Location() { }
#pragma warning restore CS8618

        public Location(PostalCode postalCode, double latitude, double longitude)
            : this(postalCode.Code, postalCode.Prefecture, postalCode.Municipality, postalCode.TownArea, postalCode.SubTownAreas, latitude, longitude) { }

        public Location(string code, string prefecture, string municipality, string? townArea, IReadOnlyList<string>? subTownAreas, double latitude, double longitude)
            => (Code, Prefecture, Municipality, TownArea, SubTownAreas, Latitude, Longitude)
            = (code, prefecture, municipality, townArea, subTownAreas, latitude, longitude);

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Location other)
            {
                return Equals(other);
            }

            return false;
        }

        public bool Equals(Location other)
        {
            // コードが同じなら他も同じということにしておく
            return Code == other.Code;
        }

        public override string ToString()
        {
            return JsonSerializer.PrettyPrint(JsonSerializer.Serialize(this));
        }

        public Point ToPoint() => new Point(Code, Latitude, Longitude);
    }
}
