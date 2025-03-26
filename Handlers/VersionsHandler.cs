using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    public static class VersionsHandler
    {
        private static char[] _PossibleSymboles { get; } = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', 'r' };

        public static VersionModel? ParseFromString(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            value = value.Trim();

            if (value.StartsWith("v") || value.StartsWith("V"))
            {
                try
                {
                    var elements = _FilterLine(value).Split(".").ToArray();

                    VersionModel result = new VersionModel();

                    try
                    {
                        result.HighNumber = int.Parse(elements[0]);
                    }
                    catch
                    {

                    }

                    try
                    {
                        result.MiddleNumber = int.Parse(elements[1]);
                    }
                    catch
                    {

                    }

                    try
                    {
                        if (elements[2].Contains(""))
                        {
                            var versionAndRevision = elements[2].Split("r").ToArray();

                            result.LowNumber = int.Parse(versionAndRevision[0]);
                            result.Revision = int.Parse(versionAndRevision[1]);
                        }
                        else
                        {
                            result.LowNumber = int.Parse(elements[2]);
                        }

                    }
                    catch
                    {

                    }

                    return result;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private static string _FilterLine(string line)
        {
            string resultLine = "";

            int dotsCount = 0;

            for (int i = 0; i < line.Length; i++)
            {
                if (_PossibleSymboles.Contains(line[i]))
                {
                    resultLine += line[i];
                }
                // Отсечение мусора псле указания версии
                else if (dotsCount == 2)
                {
                    break;
                }

                if (line[i] == '.')
                {
                    dotsCount++;
                }
            }

            return resultLine;
        }
    }
}
