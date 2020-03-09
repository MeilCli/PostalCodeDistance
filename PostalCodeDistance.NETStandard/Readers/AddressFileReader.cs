using PostalCodeDistance.NETStandard.Entities;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PostalCodeDistance.NETStandard.Readers
{
    public class AddressFileReader : IFileReader<Address>
    {
        private readonly INameTransfer municipalityNameTransfer;

        public AddressFileReader(INameTransfer municipalityNameTransfer)
            => (this.municipalityNameTransfer) = (municipalityNameTransfer);

        public IEnumerable<Address> ReadFile(string filePath)
        {
            bool isFirstLine = true;
            foreach (string line in File.ReadLines(filePath, Encoding.UTF8))
            {
                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                string[] separatedValues = line.Split('"');
                string prefecture = separatedValues[3];
                string municipality = municipalityNameTransfer.Transfer(separatedValues[7]);
                string townArea = separatedValues[11];
                double latitude = double.Parse(separatedValues[13]);
                double longitude = double.Parse(separatedValues[15]);

                yield return new Address(prefecture, municipality, townArea, latitude, longitude);
            }
        }
    }
}
