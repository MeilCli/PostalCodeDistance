using System.Collections.Generic;

namespace PostalCodeDistance.NETStandard
{
    public interface IFileReader<T>
    {
        public IEnumerable<T> ReadFile(string filePath);
    }
}
