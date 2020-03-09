namespace PostalCodeDistance.NETStandard.Readers
{
    public class MunicipalityNameTransfer : INameTransfer
    {
        public string Transfer(string name)
        {
            return name switch
            {
                "三宅島三宅村" => "三宅村",
                "八丈島八丈町" => "八丈町",
                "犬上郡大字多賀町" => "犬上郡多賀町",
                "篠山市" => "丹波篠山市",
                "筑紫郡那珂川町" => "那珂川市",
                "糟屋郡須惠町" => "糟屋郡須恵町",
                _ => name.Replace("ケ", "ヶ")
            };
        }
    }
}
