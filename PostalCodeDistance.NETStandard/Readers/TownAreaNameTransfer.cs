using System.Text;

namespace PostalCodeDistance.NETStandard.Readers
{
    public class TownAreaNameTransfer : INameTransfer
    {
        // 4桁まで変換する
        private readonly char[] digits = { '十', '百', '千' };

        // 全角数字を漢数字に変換する
        public string Transfer(string name)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < name.Length; i++)
            {
                char current = name[i];
                if (isNumber(current) is false)
                {
                    sb.Append(current);
                    continue;
                }

                int number = toNumber(current);
                if (number <= 0)
                {
                    continue;
                }

                int digitLevel = 0;
                if (i + 1 < name.Length && isNumber(name[i + 1]))
                {
                    digitLevel += 1;
                    if (i + 2 < name.Length && isNumber(name[i + 2]))
                    {
                        digitLevel += 1;
                        if (i + 3 < name.Length && isNumber(name[i + 3]))
                        {
                            digitLevel += 1;
                        }
                    }
                }

                if (0 < digitLevel)
                {
                    if (number != 1)
                    {
                        sb.Append(toKanjiNumberChar(number));
                        sb.Append(digits[digitLevel - 1]);
                    }
                    else
                    {
                        sb.Append(digits[digitLevel - 1]);
                    }
                }
                else
                {
                    sb.Append(toKanjiNumberChar(number));
                }
            }

            return sb.ToString();
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

        private int toNumber(char c)
        {
            return c switch
            {
                '０' => 0,
                '１' => 1,
                '２' => 2,
                '３' => 3,
                '４' => 4,
                '５' => 5,
                '６' => 6,
                '７' => 7,
                '８' => 8,
                '９' => 9,
                _ => -1
            };
        }

        private char toKanjiNumberChar(int value)
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
    }
}
