using CommitContentCreater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    public class CommitHistoryExtracter
    {
        public static void HistoryFileExtraction(string pathToGitLog)
        {
            List<string> lines = OrdinaryFileHandler.ReadFileContent(pathToGitLog
                , (exception) =>
                {
                    Console.WriteLine($"! Ошибка чтения файла GitLog по пути: {pathToGitLog} !");
                });

            if (File.Exists(pathToGitLog))
            {
                File.Delete(pathToGitLog);
            }

            if (lines.Count == 0)
            {
                return;
            }

            List<CommitModel> commits = _GenerateCommitsList(lines);

            List<string> historyFileContent = new List<string>();

            for (int i = commits.Count - 1; i >= 0; i--)
            {
                historyFileContent.AddRange(CommitHandler.GetHistoryFileCommitContent(commits[i]));
            }

            string gitHistoryPath = GetHistoryFilePath(pathToGitLog);

            OrdinaryFileHandler.WriteFileContent(gitHistoryPath, false, historyFileContent, null);

            Console.WriteLine($"Был создан файл истории коммитов по пути: {gitHistoryPath}");
        }

        private static string GetHistoryFilePath(string pathToGitLog)
        {
            // Формирование имени выходного файла
            var pathElements = pathToGitLog.Replace("\\", "~").Replace("/", "~").Split("~");
            string gitHistoryPath = string.Join("\\", pathElements.Take(pathElements.Count() - 1));
            if (string.IsNullOrEmpty(gitHistoryPath))
            {
                gitHistoryPath = ".";
            }
            gitHistoryPath += "\\gitHistory.txt";

            return gitHistoryPath;
        }

        private static List<CommitModel> _GenerateCommitsList(List<string> fileLines)
        {
            List<CommitModel> commits = new List<CommitModel>();

            CommitModel currentCommit = null;
            bool newVersion = true;

            foreach (var line in fileLines)
            {
                var _line = line.Trim();

                if (_line.StartsWith("Merge"))
                {
                    // Ничего не делаем
                }
                else if (_line.StartsWith("commit"))
                {
                    if (currentCommit != null)
                    {
                        if (currentCommit.Version == null)
                        {
                            if (commits.Count > 0)
                            {
                                newVersion = false;
                            }
                            else
                            {
                                newVersion = true;
                                currentCommit.Version = new VersionModel() { MiddleNumber = 1 };
                            }
                        }

                        if (newVersion)
                        {
                            commits.Add(currentCommit);
                        }
                        else
                        {
                            commits.Last().StringDate = currentCommit.StringDate;
                            foreach (var currentCommitLine in currentCommit.Lines)
                            {
                                commits.Last().AppendLine(currentCommitLine);
                            }
                        }
                    }

                    currentCommit = new CommitModel();
                }
                else if (_line.StartsWith("Author:"))
                {
                    currentCommit.Auther = _line.Replace("Author:", "").Split('<')[0].Trim();
                }
                else if (_line.StartsWith("Date:"))
                {
                    currentCommit.StringDate = _line.Replace("Author:", "").Split('<')[0].Trim();
                }
                else if (_line.StartsWith("v") || _line.StartsWith("V"))
                {
                    currentCommit.Version = VersionsHandler.ParseFromString(_line);

                    newVersion = true;
                }
                else if (!string.IsNullOrEmpty(_line))
                {
                    currentCommit.AppendLine(CommitLineHandler.CreateCommitLine(_HandleGitLogCommitLine(line)));
                }
            }

            _NormilizeHistoryCommits(commits);

            return commits;
        }

        private static void _NormilizeHistoryCommits(List<CommitModel> historyModels)
        {
            // Нормирование номеров версий ПО
            for (int i = 1; i < historyModels.Count;)
            {
                if (historyModels[i].Version.Compare(historyModels[i - 1].Version) >= 0)
                {
                    historyModels[i].StringDate = historyModels[i - 1].StringDate;
                    foreach (var currentCommitLine in historyModels[i - 1].Lines)
                    {
                        historyModels[i].AppendLine(currentCommitLine);
                    }

                    historyModels.RemoveAt(i - 1);
                }
                else
                {
                    i++;
                }
            }
        }


        private static string _HandleGitLogCommitLine(string line)
        {
            int tabsCount = -1;

            while (line.StartsWith("    ") || line.StartsWith("\t"))
            {
                if (line.StartsWith("\t"))
                {
                    line = line.Substring(1);
                }
                else
                {
                    line = line.Substring(4);
                }
                tabsCount++;
            }

            if (!line.StartsWith("-"))
            {
                line = "- " + line;
            }

            for (int i = 0; i < tabsCount; i++)
            {
                line = "-" + line;
            }

            return line;
        }

    }
}
