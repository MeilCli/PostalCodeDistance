using PostalCodeDistance.NETStandard.Entities;
using System.Collections.Generic;
using System.Linq;

namespace PostalCodeDistance.NETStandard.Locations
{
    public class LocationCalculator
    {
        public IReadOnlyList<Location> Calculate(IReadOnlyList<LocationMatch> locationMatches)
        {
            var result = new List<Location>();

            foreach (var locationMatch in locationMatches)
            {
                IReadOnlyList<Address> addresses;
                if (locationMatch.ExactMatches.Count != 0)
                {
                    addresses = locationMatch.ExactMatches;
                }
                else if (locationMatch.SameTownAreaMatches.Count != 0)
                {
                    addresses = locationMatch.SameTownAreaMatches;
                }
                else
                {
                    addresses = locationMatch.SameMunicipalitMatches;
                }

                if (addresses.Count == 0)
                {
                    continue;
                }

                double latitude = addresses.Select(x => x.Latitude).Average();
                double longitude = addresses.Select(x => x.Longitude).Average();

                result.Add(new Location(locationMatch.PostalCode, latitude, longitude));
            }

            return result;
        }
    }
}
