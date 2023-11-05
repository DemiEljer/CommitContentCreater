using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater.Models
{
    public class CommitModel
    {
        private DateTime _Date { get; set; }

        public string? StringDate { get; set; } = null;

        public DateTime Date
        {
            get => _Date;
            set
            {
                StringDate = null;
                _Date = value;
            }
        }

        public VersionModel Version { get; set; } = new VersionModel() 
            {
                HighNumber = 0,
                MiddleNumber = 0,
                LowNumber = 0
            };

        public string Auther { get; set; } = "";

        private List<CommitLineModel> _Lines { get; } = new List<CommitLineModel>();

        public CommitLineModel[] Lines => _Lines.ToArray();

        public bool IsNewVersion { get; set; } = true;

        public string GetDate()
        {
            if (string.IsNullOrEmpty(StringDate))
            {
                return Date.ToString();
            }
            else
            {
                return StringDate;
            }
        }

        public void AppendLine(CommitLineModel line)
        {
            if (line == null || !line.IsInited)
            {
                return;
            }

            for (int i = 0; i < _Lines.Count; i++)
            {
                if (_Lines[i].Priority > line.Priority)
                {
                    _Lines.Insert(i, line);
                    return;
                }
            }

            _Lines.Add(line);
        }
    }
}
