using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    public class CommitModel
    {
        public DateTime Date { get; set; }

        public string? StringDate { get; set; } = null;

        public Version? Version { get; set; } = null;

        public string Auther { get; set; } = null;

        public List<string> Lines { get; } = new List<string>();

    }
}
