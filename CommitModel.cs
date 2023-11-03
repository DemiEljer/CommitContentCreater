using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
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

        public Version Version { get; set; } = null;

        public string Auther { get; set; } = "";

        private List<CommitLineModel> _Lines { get; } = new List<CommitLineModel>();
        
        public CommitLineModel[] Lines => _Lines.ToArray();

        public void AppendLine(string line)
        {

        }

        public void AppendLine(CommitLineModel line)
        {

        }
    }
}
