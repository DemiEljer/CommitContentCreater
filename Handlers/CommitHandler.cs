using CommitContentCreater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    public class CommitHandler
    {
        public static List<string> GetHistoryFileCommitContent(CommitModel model)
        {
            List<string> resultList = new List<string>();

            if (model.IsNewVersion)
            {
                resultList.Add(model.Version.ToString());
                resultList.Add($"~~~ {model.Auther} [{model.GetDate()}] ~~~");
                resultList.Add("");

                foreach (var line in model.Lines)
                {
                    resultList.Add(line.Line);
                }

                resultList.Add("");
                resultList.Add("===============================================================================================");
                resultList.Add("");
            }
            else
            {
                resultList.Add("");
                resultList.Add($"~~~ {model.Auther} [{model.GetDate()}] ~~~");
                resultList.Add("");
                foreach (var line in model.Lines)
                {
                    resultList.Add(line.Line);
                }
            }

            return resultList;
        }

        public static List<string> GetCommitFileContent(CommitModel model)
        {
            List<string> resultList = new List<string>();

            resultList.Add(model.Version.ToString());
            resultList.Add("");

            foreach (var line in model.Lines)
            {
                resultList.Add(line.Line);
            }

            return resultList;
        }
    }
}
