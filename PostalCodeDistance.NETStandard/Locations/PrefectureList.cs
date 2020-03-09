using PostalCodeDistance.NETStandard.Entities;
using System.Collections.Generic;

namespace PostalCodeDistance.NETStandard.Locations
{
    public class PrefectureList
    {
        private readonly Dictionary<string, PrefectureSet> prefectureDictionary = new Dictionary<string, PrefectureSet>();
        private readonly List<PrefectureSet> prefectureSets = new List<PrefectureSet>();

        public IReadOnlyList<PrefectureSet> PrefectureSets => prefectureSets;

        public void Add(PostalCode postalCode)
        {
            if (prefectureDictionary.ContainsKey(postalCode.Prefecture))
            {
                prefectureDictionary[postalCode.Prefecture].Add(postalCode);
            }
            else
            {
                var prefectureSet = new PrefectureSet(postalCode.Prefecture);
                prefectureSet.Add(postalCode);
                prefectureDictionary[postalCode.Prefecture] = prefectureSet;
                prefectureSets.Add(prefectureSet);
            }
        }

        public void Add(Address address)
        {
            if (prefectureDictionary.ContainsKey(address.Prefecture))
            {
                prefectureDictionary[address.Prefecture].Add(address);
            }
            else
            {
                var prefectureSet = new PrefectureSet(address.Prefecture);
                prefectureSet.Add(address);
                prefectureDictionary[address.Prefecture] = prefectureSet;
                prefectureSets.Add(prefectureSet);
            }
        }
    }
}
