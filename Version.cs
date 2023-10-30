using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    public class Version
    {
        public int HighNumber { get; set; } = 0;

        public int MiddleNumber { get; set; } = 0;

        public int LowNumber { get; set; } = 0;

        public static Version operator +(Version origin, Version adding)
        {
            if (adding.HighNumber > 0)
            {
                origin.HighNumber += adding.HighNumber;
                origin.MiddleNumber = 0;
                origin.LowNumber = 0;
            }
            else if (adding.MiddleNumber > 0)
            {
                origin.MiddleNumber += adding.MiddleNumber;
                origin.LowNumber = 0;
            }
            else if (adding.LowNumber > 0)
            {
                origin.LowNumber += adding.LowNumber;
            }

            return origin;
        }

        public int Compare(Version another)
        {
            var diff = HighNumber - another.HighNumber;
            if (diff != 0)
            {
                return diff;
            }

            diff = MiddleNumber - another.MiddleNumber;
            if (diff != 0)
            {
                return diff;
            }

            return LowNumber - another.LowNumber;
        }

        public override string ToString()
        {
            return $"v {HighNumber}.{MiddleNumber}.{LowNumber}";
        }

        public static Version? ParseFromString(string value)
        {
            value = value.Trim();

            if (value.StartsWith("v") || value.StartsWith("V"))
            {
                try
                {
                    var elements = value.Replace("v", "").Replace("V", "").Trim().Split(".").ToArray();

                    Version result = new Version();

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
                        result.LowNumber = int.Parse(elements[2]);
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

        public Version Clone()
        {
            return new Version()
            {
                HighNumber = HighNumber,
                MiddleNumber = MiddleNumber,
                LowNumber = LowNumber
            };
        }
    }
}
