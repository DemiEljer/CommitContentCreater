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
            List<string> lines = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(pathToGitLog))
                {
                    while (!sr.EndOfStream)
                    {
                        lines.Add(sr.ReadLine());
                    }
                }
            }
            catch
            {
                Console.WriteLine($"! Ошибка чтения файла GitLog по пути: {pathToGitLog} !");

                return;
            }

            var commits = _GenerateCommitsList(lines);

            List<string> historyFileContent = new List<string>();

            for (int i = commits.Count - 1; i >= 0; i--)
            {
                historyFileContent.AddRange(FileHandling.GetHistoryFileCommit(
                    commits[i].Auther,
                    commits[i].StringDate,
                    commits[i].Lines,
                    commits[i].Version,
                    true
                    ));
            }

            // Формирование имени выходного файла
            var pathElements = pathToGitLog.Replace("\\", "~").Replace("/", "~").Split("~");
            string gitHistoryPath = string.Join("\\", pathElements.Take(pathElements.Count() - 1));
            if (string.IsNullOrEmpty(gitHistoryPath))
            {
                gitHistoryPath = ".";
            }
            gitHistoryPath += "\\gitHistory.txt";

            using (StreamWriter sr = new StreamWriter(gitHistoryPath))
            {
                foreach (var line in historyFileContent)
                {
                    sr.WriteLine(line);
                }
            }

            Console.WriteLine($"Был создан файл истории коммитов по пути: {gitHistoryPath}");
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
                                currentCommit.Version = new Version() { MiddleNumber = 1 };
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
                                commits.Last().Lines.Add(currentCommitLine);
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
                    currentCommit.Version = Version.ParseFromString(_line);

                    newVersion = true;
                }
                else if (!string.IsNullOrEmpty(_line))
                {
                    currentCommit.Lines.Add(FileHandling.HandleCommitLine(_HandleGitLogCommitLine(line)));
                }
            }

            // Нормирование номеров версий ПО
            for (int i = 1; i < commits.Count; )
            {
                if (commits[i].Version.Compare(commits[i - 1].Version) >= 0)
                {
                    commits[i].StringDate = commits[i - 1].StringDate;
                    foreach (var currentCommitLine in commits[i - 1].Lines)
                    {
                        commits[i].Lines.Add(currentCommitLine);
                    }

                    commits.RemoveAt(i - 1);
                }
                else
                {
                    i++;
                }
            }

            return commits;
        }

        private static string _HandleGitLogCommitLine(string line)
        {
            int tabsCount = -1;

            while (line.StartsWith("    ") || line.StartsWith("\t")) 
            {
                if (line.StartsWith("\t"))
                {
                    line = line.Substring(1, line.Length - 1);
                }
                else
                {
                    line = line.Substring(4, line.Length - 4);
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
