using PostalCodeDistance.NETStandard.Entities;
using System.Collections.Generic;

namespace PostalCodeDistance.NETStandard.Locations
{
    public class PrefectureSet
    {
        private readonly List<PostalCode> postalCodes = new List<PostalCode>();
        private readonly List<Address> addresses = new List<Address>();

        public string Prefecture { get; }

        public IReadOnlyList<PostalCode> PostalCodes => postalCodes;

        public IReadOnlyList<Address> Addresses => addresses;

        public PrefectureSet(string prefecture) => Prefecture = prefecture;

        public void Add(PostalCode postalCode)
        {
            postalCodes.Add(postalCode);
        }

        public void Add(Address address)
        {
            addresses.Add(address);
        }
    }
}
