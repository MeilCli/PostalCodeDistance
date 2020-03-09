using PostalCodeDistance.NETStandard.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PostalCodeDistance.NETStandard.Readers
{
    public class PostalCodeFileReader : IFileReader<PostalCode>
    {
        private readonly INameTransfer municipalityNameTransfer;
        private readonly INameTransfer townAreaNameTransfer;

        public PostalCodeFileReader(INameTransfer municipalityNameTransfer, INameTransfer townAreaNameTransfer)
            => (this.municipalityNameTransfer, this.townAreaNameTransfer) = (municipalityNameTransfer, townAreaNameTransfer);

        public IEnumerable<PostalCode> ReadFile(string filePath)
        {
            foreach (string line in File.ReadLines(filePath, Encoding.UTF8))
            {
                string[] separatedValues = line.Split('"');
                string code = separatedValues[3];
                string prefecture = separatedValues[11];
                string municipality = municipalityNameTransfer.Transfer(separatedValues[13]);
                string? townArea = separatedValues[15];
                IReadOnlyList<string>? subTownArea = null;
                if (townArea == "以下に掲載がない場合")
                {
                    townArea = null;
                }
                else
                {
                    (townArea, subTownArea) = parseTownArea(townArea);
                    townArea = townAreaNameTransfer.Transfer(townArea);
                }

                yield return new PostalCode(code, prefecture, municipality, townArea, subTownArea);
            }
        }

        private (string, IReadOnlyList<string>?) parseTownArea(string townArea)
        {
            int startIndex = townArea.IndexOf('（');
            int endIndex = townArea.IndexOf('）');
            if (startIndex < 0 || endIndex < 0 || endIndex + 1 < startIndex)
            {
                return (townArea, null);
            }

            string townAreaResult = townArea.Substring(0, startIndex);
            string subTownAreaContent = townArea.Substring(startIndex + 1, endIndex - 1 - startIndex);

            if (subTownAreaContent == "その他")
            {
                return (townAreaResult, null);
            }

            IReadOnlyList<string>? subTownAreaResult = parseSubTownArea(subTownAreaContent)
                .SelectMany(x => filterAndSelectSubTownArea(x))
                .Select(x => townAreaNameTransfer.Transfer(x))
                .ToList();
            if (subTownAreaResult?.Count == 0)
            {
                subTownAreaResult = null;
            }

            return (townAreaResult, subTownAreaResult);
        }

        // 区切り方がやばい
        // ３５～３８、４１、４２丁目
        // ２４８、３３９、７２６、７８０、８００、８０６番地
        // 西５～８線７９～１１０番地
        // 上勇知、下勇知、夕来、オネトマナイ
        // １１３～７９１番地、西活込、西上、西中
        private IReadOnlyList<string> parseSubTownArea(string subTownArea)
        {
            var reversedSubTownArea = new string(subTownArea.Reverse().ToArray());
            var result = new List<string>();
            var current = new StringBuilder();
            bool isCommaNotSeparated = false;

            for (int i = 0; i < reversedSubTownArea.Length; i++)
            {
                if (reversedSubTownArea[i] == '目' && i + 1 < reversedSubTownArea.Length && reversedSubTownArea[i + 1] == '丁')
                {
                    isCommaNotSeparated = true;
                }
                if (reversedSubTownArea[i] == '地' && i + 1 < reversedSubTownArea.Length && reversedSubTownArea[i + 1] == '番')
                {
                    isCommaNotSeparated = true;
                }

                if (reversedSubTownArea[i] == '、')
                {
                    if (isCommaNotSeparated)
                    {
                        if (i + 1 < reversedSubTownArea.Length && isNumber(reversedSubTownArea[i + 1]))
                        {
                            current.Append(reversedSubTownArea[i]);
                        }
                        else
                        {
                            result.Add(new string(current.ToString().Reverse().ToArray()));
                            current.Clear();
                            isCommaNotSeparated = false;
                        }
                    }
                    else
                    {
                        result.Add(new string(current.ToString().Reverse().ToArray()));
                        current.Clear();
                        isCommaNotSeparated = false;
                    }
                }
                else
                {
                    current.Append(reversedSubTownArea[i]);
                }
            }
            if (0 < current.Length)
            {
                result.Add(new string(current.ToString().Reverse().ToArray()));
            }

            return result;
        }

        private bool isNumber(char c)
        {
            return c switch
            {
                '０' => true,
                '１' => true,
                '２' => true,
                '３' => true,
                '４' => true,
                '５' => true,
                '６' => true,
                '７' => true,
                '８' => true,
                '９' => true,
                _ => false
            };
        }

        private bool isOnlyNumberAndSymbol(string value)
        {
            foreach (char c in value)
            {
                if (isNumber(c) is false && c != '－' && c != '～' && c != '、')
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<string> filterAndSelectSubTownArea(string subTownArea)
        {
            if (subTownArea.EndsWith("番地"))
            {
                yield break;
            }
            if (subTownArea == "その他")
            {
                yield break;
            }
            if (isOnlyNumberAndSymbol(subTownArea))
            {
                yield break;
            }
            if (subTownArea.EndsWith("丁目") is false)
            {
                yield return subTownArea;
                yield break;
            }

            string subTownAreaValue = subTownArea.Substring(0, subTownArea.Length - 2);
            string[] subTownAreaValues = subTownAreaValue.Split('、');

            foreach (string value in subTownAreaValues)
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                if (value.Contains('～') is false)
                {
                    int number = int.Parse(toLowerCaseNumberString(value));
                    yield return toKanjiNumber(number) + "丁目";
                    continue;
                }

                string[] separatedValues = value.Split('～');

                if (separatedValues.Length < 2)
                {
                    continue;
                }

                int startNumber = int.Parse(toLowerCaseNumberString(separatedValues[0]));
                int endNumber = int.Parse(toLowerCaseNumberString(separatedValues[1]));

                if (endNumber <= startNumber)
                {
                    continue;
                }

                for (int i = startNumber; i <= endNumber; i++)
                {
                    yield return toKanjiNumber(i) + "丁目";
                }
            }
        }

        // 丁目の最大値まで対応する
        // 3桁まではいかない
        private string toKanjiNumber(int value)
        {
            char toKanjiNumberChar(int value)
            {
                return value switch
                {
                    1 => '一',
                    2 => '二',
                    3 => '三',
                    4 => '四',
                    5 => '五',
                    6 => '六',
                    7 => '七',
                    8 => '八',
                    9 => '九',
                    _ => ' '
                };
            }

            if (value <= 0)
            {
                return string.Empty;
            }
            if (value < 10)
            {
                return toKanjiNumberChar(value).ToString();
            }

            int i = value % 10;
            int j = value / 10;
            var sb = new StringBuilder();
            if (1 < j)
            {
                sb.Append(toKanjiNumberChar(j));
                sb.Append('十');
            }
            if (j == 1)
            {
                sb.Append('十');
            }
            if (i != 0)
            {
                sb.Append(toKanjiNumber(i));
            }
            return sb.ToString();
        }

        private string toLowerCaseNumberString(string numberString)
        {
            static char toLowerCase(char c)
            {
                return c switch
                {
                    '０' => '0',
                    '１' => '1',
                    '２' => '2',
                    '３' => '3',
                    '４' => '4',
                    '５' => '5',
                    '６' => '6',
                    '７' => '7',
                    '８' => '8',
                    '９' => '9',
                    _ => ' '
                };
            }

            return new string(numberString.Select(x => toLowerCase(x)).ToArray());
        }
    }
}
