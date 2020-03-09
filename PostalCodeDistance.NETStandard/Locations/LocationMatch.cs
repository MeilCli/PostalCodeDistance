using PostalCodeDistance.NETStandard.Entities;
using System.Collections.Generic;

namespace PostalCodeDistance.NETStandard.Locations
{
    public class LocationMatch
    {
        private readonly List<Address> exactMatches = new List<Address>();
        private readonly List<Address> sameTownAreaMatches = new List<Address>();
        private readonly List<Address> sameMunicipalityMatches = new List<Address>();

        public PostalCode PostalCode { get; }

        public IReadOnlyList<Address> ExactMatches => exactMatches;

        public IReadOnlyList<Address> SameTownAreaMatches => sameTownAreaMatches;

        public IReadOnlyList<Address> SameMunicipalitMatches => sameMunicipalityMatches;

        public LocationMatch(PostalCode postalCode) => PostalCode = postalCode;

        public void AddExactMatch(Address address)
        {
            exactMatches.Add(address);
        }

        public void AddSameTownAreaMatch(Address address)
        {
            sameTownAreaMatches.Add(address);
        }

        public void AddSameMunicipalityMatches(Address address)
        {
            sameMunicipalityMatches.Add(address);
        }
    }
}
