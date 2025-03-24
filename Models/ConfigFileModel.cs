using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater.Models
{
    internal class ConfigFileModel
    {
        public const string FileExtentionsKeyWord = "extentions";
        public const string FilesKeyWord = "files";
        public const string ProjectPathKeyWord = "project";
        public const string ClearCommitsKeyWord = "clear";
        public const string ShowTracingKeyWord = "tracing";

        public List<string> FileExtentions { get; } = new List<string>();

        public List<string> Files { get; } = new List<string>();

        public string ProjectPath { get; set; } = "";

        public bool ClearCommitLines { get; set; } = false;

        public bool ShowTracing { get; set; } = false;
    }
}
