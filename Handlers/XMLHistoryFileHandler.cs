using CommitContentCreater.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CommitContentCreater.Handlers
{
    internal class XMLHistoryFileHandler
    {
        public static void GenerateHistoryFile(string path)
        {
            string pathToFile = OrdinaryFileHandler.GetPathToFile(path, "history.txt");

            List<string> historyFileContent = OrdinaryFileHandler.ReadFileContent(pathToFile, null);

            var commits = _GetCommitsList(historyFileContent, (message) =>
            {
                LoggingHandler.LogError(message);   
            });

            string pathToResultFile = OrdinaryFileHandler.GetPathToFile(path, "history.xml");

            _SaveXMLFile(commits, pathToResultFile);

            Console.WriteLine($"Был сгенерирован файл историчности по пути: {pathToResultFile}");
        }

        private static List<CommitModel> _GetCommitsList(List<string> historyFileContent, Action<string> errorMessageCallback)
        {
            List<CommitModel> resultList = new List<CommitModel>();

            CommitModel? currentCommit = null;

            for (int i = 0; i < historyFileContent.Count; i++)
            {
                var historyFileLine = historyFileContent[i];

                var version = VersionsHandler.ParseFromString(historyFileLine);

                if (version != null)
                {
                    if (currentCommit != null)
                    {
                        resultList.Add(currentCommit);
                    }

                    currentCommit = new CommitModel();
                    currentCommit.Version = version;
                }
                else if (historyFileLine.Trim().StartsWith("~~~"))
                {
                    if (currentCommit == null)
                    {
                        currentCommit = new CommitModel();
                    }

                    currentCommit.StringDate = DateParser.GetNormalizedDate(historyFileLine.Trim().Split("[")[1].Split("]")[0], (message) =>
                    {
                        errorMessageCallback.Invoke(message);
                    });
                }
                else if (!string.IsNullOrEmpty(historyFileLine.Trim())
                         && !historyFileLine.Trim().StartsWith("="))
                {
                    if (currentCommit == null)
                    {
                        currentCommit = new CommitModel();
                    }

                    currentCommit.AppendLine(new CommitLineModel()
                    {
                        Line = historyFileLine
                    });
                }
            }

            if (currentCommit != null)
            {
                resultList.Add(currentCommit);
            }

            return resultList;
        }
    
        private static void _SaveXMLFile(List<CommitModel> commits, string pathToResultFile)
        {
            XmlDocument resultDoc = new XmlDocument();
            var resultBody = resultDoc.CreateElement("versions");
            resultDoc.AppendChild(resultBody);

            foreach (var commit in commits)
            {
                var version = resultDoc.CreateElement("version");
                resultBody.AppendChild(version);

                version.SetAttribute("Version", commit.Version.ToString());
                version.SetAttribute("Date", commit.StringDate?.Split(" ")[0]);
                version.SetAttribute("Description", string.Join("\r\n", commit.Lines.Select(l => l.Line)));
            }

            resultDoc.Save(pathToResultFile);
        }
    }
}
