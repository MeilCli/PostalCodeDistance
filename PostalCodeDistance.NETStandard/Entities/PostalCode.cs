using System.Collections.Generic;

namespace PostalCodeDistance.NETStandard.Entities
{
    public class PostalCode
    {
        public string Code { get; }

        public string Prefecture { get; }

        public string Municipality { get; }

        public string? TownArea { get; }

        public IReadOnlyList<string>? SubTownAreas { get; }

        public PostalCode(string code, string prefecture, string municipality, string? townArea, IReadOnlyList<string>? subTownAreas)
            => (Code, Prefecture, Municipality, TownArea, SubTownAreas) = (code, prefecture, municipality, townArea, subTownAreas);

        public override string ToString()
        {
            return $"Code: {Code}, Prefecture: {Prefecture}, Municipality: {Municipality}, TownArea: {TownArea}";
        }
    }
}
