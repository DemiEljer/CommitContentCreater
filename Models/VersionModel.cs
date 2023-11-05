using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    public class VersionModel
    {
        public int HighNumber { get; set; } = 0;

        public int MiddleNumber { get; set; } = 0;

        public int LowNumber { get; set; } = 0;

        public bool IsEmpty =>
            HighNumber == 0
            && MiddleNumber == 0
            && LowNumber == 0;

        public static VersionModel operator +(VersionModel origin, VersionModel adding)
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

        public int Compare(VersionModel another)
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

        public VersionModel Clone()
        {
            return new VersionModel()
            {
                HighNumber = HighNumber,
                MiddleNumber = MiddleNumber,
                LowNumber = LowNumber
            };
        }
    }
}
