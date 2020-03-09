using System.Collections.Generic;

namespace PostalCodeDistance.NETStandard.Locations
{
    public class LocationMatcher
    {
        public IReadOnlyList<LocationMatch> FindLocations(PrefectureList prefectureList)
        {
            var result = new List<LocationMatch>();

            foreach (var prefectureSet in prefectureList.PrefectureSets)
            {
                result.AddRange(FindLocations(prefectureSet));
            }

            return result;
        }

        public IReadOnlyList<LocationMatch> FindLocations(PrefectureSet prefectureSet)
        {
            var result = new List<LocationMatch>();

            foreach (var postalCode in prefectureSet.PostalCodes)
            {
                var locationMatch = new LocationMatch(postalCode);

                foreach (var address in prefectureSet.Addresses)
                {
                    if (postalCode.Prefecture != address.Prefecture
                        || postalCode.Municipality != address.Municipality)
                    {
                        continue;
                    }

                    locationMatch.AddSameMunicipalityMatches(address);

                    if (postalCode.TownArea is null)
                    {
                        continue;
                    }
                    if (address.TownArea.StartsWith(postalCode.TownArea))
                    {
                        locationMatch.AddSameTownAreaMatch(address);
                    }

                    if (postalCode.SubTownAreas is null)
                    {
                        continue;
                    }
                    foreach (string subTownArea in postalCode.SubTownAreas)
                    {
                        if (address.TownArea == (postalCode.TownArea + subTownArea))
                        {
                            locationMatch.AddExactMatch(address);
                        }
                    }
                }

                result.Add(locationMatch);
            }

            return result;
        }
    }
}
