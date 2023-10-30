using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    public class FileHandling
    {
        private enum CommitLineType
        {
            NotACommitLine,
            SingleCommitLine,
            StartOfCommitLines,
            EndOfCommitLines
        }

        private enum FileParsingState
        {
            OrdinaryReading,
            MulltilineCommentReading
        }

        public static List<string> FindCommitLines(string pathToFile, bool cleanFilesFlag)
        {
            List<string> resultLines = new List<string>();
            List<string> fileLines = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(pathToFile))
                {
                    FileParsingState readingState = FileParsingState.OrdinaryReading;

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        var lineType = _IsCommitLine(line);

                        var prevReadingState = readingState;

                        switch (readingState)
                        {
                            case FileParsingState.OrdinaryReading:
                                if (lineType == CommitLineType.StartOfCommitLines)
                                {
                                    readingState = FileParsingState.MulltilineCommentReading;
                                }
                                break;

                            case FileParsingState.MulltilineCommentReading:
                                if (lineType == CommitLineType.EndOfCommitLines)
                                {
                                    readingState = FileParsingState.OrdinaryReading;
                                }
                                break;
                        }


                        if (readingState == FileParsingState.MulltilineCommentReading
                            || lineType == CommitLineType.SingleCommitLine
                            || lineType == CommitLineType.StartOfCommitLines
                            || (lineType == CommitLineType.EndOfCommitLines && prevReadingState == FileParsingState.MulltilineCommentReading))
                        {
                            var commitLine = HandleCommitLine(line);

                            if (!string.IsNullOrEmpty(commitLine))
                            {
                                resultLines.Add(commitLine);
                            }
                        }
                        else
                        {
                            fileLines.Add(line);
                        }

                        prevReadingState = readingState;
                    }
                }
                // Если был сформирован файл
                if (resultLines.Count > 0 && cleanFilesFlag)
                {
                    using (StreamWriter sw = new StreamWriter(pathToFile))
                    {
                        foreach (var line in fileLines)
                        {
                            sw.WriteLine(line);
                        }
                    }
                }


            }
            catch
            {

            }

            return resultLines;
        }

        private static CommitLineType _IsCommitLine(string originLine)
        {
            if (originLine.Contains("//~"))
            {
                return CommitLineType.SingleCommitLine;
            }
            else if (originLine.Contains("/*~"))
            {
                return CommitLineType.StartOfCommitLines;
            }
            else if (originLine.Contains("*/") || originLine.Contains("~*/"))
            {
                return CommitLineType.EndOfCommitLines;
            }
            else
            {
                return CommitLineType.NotACommitLine;
            }
        }

        public static string HandleCommitLine(string originLine)
        {
            int tabsCount = originLine.Where(s => s == '\t').Count();

            var resultString = originLine.Replace("//~", "").Replace("/*~", "").Replace("~*/", "").Replace("*/", "").Replace("*", "").Trim();

            if (!resultString.StartsWith("-"))
            {
                if (!string.IsNullOrEmpty(resultString) && !resultString.StartsWith("v"))
                {
                    resultString = "- " + resultString;
                }
            }
            else
            {
                string tabs = "";

                while (resultString.StartsWith("--"))
                {
                    resultString = resultString.Substring(1, resultString.Length - 1);
                    tabs += "\t";
                }

                resultString = tabs + resultString;
            }

            return CommitLineModification(resultString);
        }

        public static void GenerateCommitFile(string path, List<string> lines, Version version)
        {
            string pathToFile = path;

            if (!path.EndsWith("\\"))
            {
                pathToFile += "\\";
            }
            pathToFile += "commit.txt";

            using (StreamWriter sw = new StreamWriter(pathToFile))
            {
                sw.WriteLine(version.ToString());
                sw.WriteLine("");

                foreach (var line in lines)
                {
                    sw.WriteLine(line);
                }
            }

            Console.WriteLine($"Был сгенерирован файл коммита по пути: {pathToFile}");
        }

        public static Version FindVersionInHistoryFile(string path)
        {
            string pathToFile = path;

            if (!path.EndsWith("\\"))
            {
                pathToFile += "\\";
            }
            pathToFile += "history.txt";

            try
            {
                Version resultVersion = new Version();

                using (StreamReader sr = new StreamReader(pathToFile))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine().Trim();

                        if (line.StartsWith("v") || line.StartsWith("V"))
                        {
                            resultVersion = Version.ParseFromString(line);
                        }
                    }
                }

                if (resultVersion.HighNumber == 0
                    && resultVersion.MiddleNumber == 0
                    && resultVersion.LowNumber == 0)
                {
                    resultVersion.MiddleNumber = 1;
                }

                return resultVersion;
            }
            catch
            {
                return new Version()
                {
                    MiddleNumber = 1
                };
            }
        }

        public static void AppendHistoryFile(string path, List<string> lines, Version version, bool isNewVersion)
        {
            string pathToFile = path;

            if (!path.EndsWith("\\"))
            {
                pathToFile += "\\";
            }
            pathToFile += "history.txt";

            List<string> historyFileContent = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(pathToFile))
                {
                    while (!sr.EndOfStream)
                    {
                        historyFileContent.Add(sr.ReadLine());
                    }
                }
            }
            catch
            {

            }

            string userName = Environment.UserName;

            List<string> newHsitoryFileContent = GetHistoryFileCommit(userName
                , DateTime.Now.ToString()
                , lines, version
                , historyFileContent.Count == 0
                  || isNewVersion);

            if (historyFileContent.Count == 0
                || isNewVersion)
            {
                historyFileContent.AddRange(newHsitoryFileContent);
            }
            else
            {
                historyFileContent.InsertRange(historyFileContent.Count - 3, newHsitoryFileContent);
            }

            using (StreamWriter sw = new StreamWriter(pathToFile))
            {
                foreach (var line in historyFileContent)
                {
                    sw.WriteLine(line);
                }
            }

            Console.WriteLine($"Был дополнен файл истории коммитов по пути: {pathToFile}");
        }

        public static List<string> GetHistoryFileCommit(string auther, string date, List<string> lines, Version version, bool isNewVersion)
        {
            List<string> resultList = new List<string>();

            if (isNewVersion)
            {
                resultList.Add(version.ToString());
                resultList.Add($"~~~ {auther} [{date}] ~~~");
                resultList.Add("");

                foreach (var line in lines)
                {
                    resultList.Add(line);
                }

                resultList.Add("");
                resultList.Add("===============================================================================================");
                resultList.Add("");
            }
            else
            {
                resultList.Add("");
                resultList.Add($"~~~ {auther} [{date}] ~~~");
                resultList.Add("");
                foreach (var line in lines)
                {
                    resultList.Add(line);
                }
            }

            return resultList;
        }

        public static string CommitLineModification(string commitLine)
        {
            Func<string, string, string, string> modifierDelegate = (string line, string key, string value) =>
            {
                return line
                .Replace($"#{key.ToLower()}", $"[{value.ToUpper()}]")
                .Replace($"#{key.ToUpper()}", $"[{value.ToUpper()}]")
                ;
            };

            commitLine = modifierDelegate(commitLine, "f", "FIX");
            commitLine = modifierDelegate(commitLine, "m", "MODIFY");
            commitLine = modifierDelegate(commitLine, "e", "EVENT");
            commitLine = modifierDelegate(commitLine, "u", "UPDATE");
            commitLine = modifierDelegate(commitLine, "a", "ADD");
            commitLine = modifierDelegate(commitLine, "i", "INFORMATION");
            commitLine = modifierDelegate(commitLine, "n", "NOTE");

            return commitLine;
        }
    
        
    }
}
