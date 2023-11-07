using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommitContentCreater.Handlers;
using CommitContentCreater.Models;

namespace CommitContentCreater
{
    public class CommitFileHander
    {
        public static void FindCommitLines(CommitModel commitModel, string pathToFile, bool cleanFilesFlag)
        {
            List<string> fileLines = new List<string>();

            CommitLineExtracter parser = new CommitLineExtracter();

            OrdinaryFileHandler.ReadFileContent(pathToFile, (lineIndex, line) =>
            {
                CommitLineModel commitLine = parser.ExtractLine(line);

                if (commitLine != null)
                {
                    if (commitLine.IsVersion)
                    {
                        var newVersion = VersionsHandler.ParseFromString(commitLine.Line);
                        if (newVersion != null && newVersion.Compare(commitModel.Version) > 0)
                        {
                            commitModel.Version = newVersion;
                        }
                    }
                    else
                    {
                        commitModel.AppendLine(commitLine);
                    }
                }
                else
                {
                    fileLines.Add(line);
                }
            },
            (exception) =>
            {

            });

            // Если был сформирован файл
            if (commitModel.Lines.Length > 0 && cleanFilesFlag)
            {
                OrdinaryFileHandler.WriteFileContent(pathToFile, false, fileLines, null);
            }
        }

        public static void GenerateCommitFile(string path, CommitModel model)
        {
            string pathToFile = OrdinaryFileHandler.GetPathToFile(path, "commit.txt");

            List<string> fileContent = CommitHandler.GetCommitFileContent(model);

            OrdinaryFileHandler.WriteFileContent(pathToFile, false, fileContent, null);

            Console.WriteLine($"Был сгенерирован файл коммита по пути: {pathToFile}");
        }

        public static VersionModel FindVersionInHistoryFile(string path)
        {
            string pathToFile = OrdinaryFileHandler.GetPathToFile(path, "history.txt");

            VersionModel resultVersion = null;

            OrdinaryFileHandler.ReadFileContent(pathToFile, (lineIndex, line) =>
            {
                line = line.Trim();

                if (line.StartsWith("v") || line.StartsWith("V"))
                {
                    resultVersion = VersionsHandler.ParseFromString(line);
                }
            }
            , (exception) =>
            {

            });

            return resultVersion == null ? new VersionModel() : resultVersion;
        }

        public static void AppendHistoryFile(string path, CommitModel model)
        {
            string pathToFile = OrdinaryFileHandler.GetPathToFile(path, "history.txt");

            List<string> historyFileContent = OrdinaryFileHandler.ReadFileContent(pathToFile, null);

            string userName = Environment.UserName;

            List<string> newHsitoryFileContent = CommitHandler.GetHistoryFileCommitContent(model);

            if (historyFileContent.Count == 0
                || model.IsNewVersion)
            {
                historyFileContent.AddRange(newHsitoryFileContent);
            }
            else
            {
                historyFileContent.InsertRange(historyFileContent.Count - 3, newHsitoryFileContent);
            }

            OrdinaryFileHandler.WriteFileContent(pathToFile, false, historyFileContent, null);

            Console.WriteLine($"Был дополнен файл истории коммитов по пути: {pathToFile}");
        }
    }
}
