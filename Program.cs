using System;
using System.Linq;

bool cleanFilesFlag = false;
string projectDirrectory = ".\\";
List<string> fileExtentions = new List<string>();

bool extentionReading = false;
bool helpShow = false;
for (int i = 0; i < args.Length; i++)
{
    try
    {
        if (args[i] == "-c")
        {
            cleanFilesFlag = true;
        }
        else if (args[i] == "-e")
        {
            extentionReading = true;
        }
        else if (args[i] == "-h")
        {
            helpShow = true;
        }
        else if (!string.IsNullOrEmpty(args[i]))
        {
            if (extentionReading)
            {
                fileExtentions.Add(args[i]);
                extentionReading = false;
            }
            else
            {
                projectDirrectory = args[i];
            }
        }
        else
        {
            Console.WriteLine("! Ошибка чтения аргумента !");
            helpShow = true;
        }
    }
    catch
    {

    }
}

if (helpShow)
{
    Console.WriteLine("CommitContentCreater v 1.0.0");
    Console.WriteLine("-h               : Выводит справочную ифнормацию по утилите");
    Console.WriteLine("-c               : Флаг очистка проекта от строк с описанием коммита");
    Console.WriteLine("-e <extention>   : Указание допустимое расширения файла для поиска коммита (-e h -e c)");
    Console.WriteLine("<Path>           : Путь к директории указывается просто как строка");
}

List<string> resultLines = new List<string>();

Version currentVersion = FileHandling.FindVersionInHistoryFile(projectDirrectory);

try
{
    foreach (var filePath in Directory.GetFiles(projectDirrectory, "*.*", SearchOption.AllDirectories))
    {
        string extention = filePath.Split('.').Last();

        if (fileExtentions.Contains(extention))
        {
            resultLines.AddRange(FileHandling.FindCommitLines(filePath, cleanFilesFlag));
        }
    }
}
catch
{
    Console.WriteLine("! Ошибка доступа к директории проекта !");
}

if (currentVersion.HighNumber == 0 
    && currentVersion.MiddleNumber == 0
    && currentVersion.LowNumber == 0 )
{
    currentVersion.MiddleNumber = 1;
}

// Поиск модификатора версии в файлах
Version versionModifier = new Version() { LowNumber = 1 };
for (int i = 0; i < resultLines.Count;)
{
    var _versionModifier = Version.ParseFromString(resultLines[i]);

    if (_versionModifier != null)
    {
        versionModifier = _versionModifier;
        resultLines.RemoveAt(i);
    }
    else
    {
        i++;
    }
}

// Модификация версии
currentVersion += versionModifier;

if (resultLines.Count != 0)
{
    FileHandling.GenerateCommitFile(projectDirrectory, resultLines, currentVersion);
    FileHandling.AppendHistoryFile(projectDirrectory, resultLines, currentVersion);
}
else
{
    Console.WriteLine("! Не было найдено ни одной строки коммита !");
}

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
                        var commitLine = _HandleLine(line);

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

    private static string _HandleLine(string originLine)
    {
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

            return resultVersion;
        }
        catch
        {
            return new Version();
        }
    }

    public static void AppendHistoryFile(string path, List<string> lines, Version version)
    {
        string pathToFile = path;

        if (!path.EndsWith("\\"))
        {
            pathToFile += "\\";
        }
        pathToFile += "history.txt";

        using (StreamWriter sw = new StreamWriter(pathToFile, true))
        {
            sw.WriteLine(version.ToString());
            sw.WriteLine("");

            foreach (var line in lines)
            {
                sw.WriteLine(line);
            }

            sw.WriteLine();
            sw.WriteLine("----------------------------------------------------------------------------------------------");
            sw.WriteLine();
        }

        Console.WriteLine($"Был дополнен файл истории коммитов по пути: {pathToFile}");
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

public class Version
{
    public int HighNumber { get; set; } = 0;

    public int MiddleNumber { get; set; } = 0;
    
    public int LowNumber { get; set; } = 0;

    public static Version operator+(Version origin, Version adding)
    {
        origin.HighNumber += adding.HighNumber;
        origin.MiddleNumber += adding.MiddleNumber;
        origin.LowNumber += adding.LowNumber;

        return origin;
    }

    public override string ToString()
    {
        return $"v {HighNumber}.{MiddleNumber}.{LowNumber}";
    }

    public static Version? ParseFromString(string value)
    {
        value = value.Trim();

        if (value.StartsWith("v") || value.StartsWith("V"))
        {
            try
            {
                var elements = value.Replace("v", "").Replace("V", "").Trim().Split(".").ToArray();

                Version result = new Version();

                try
                {
                    result.HighNumber = int.Parse(elements[0]);
                }
                catch
                {

                }

                try
                {
                    result.MiddleNumber = int.Parse(elements[1]);
                }
                catch
                {

                }

                try
                {
                    result.LowNumber = int.Parse(elements[2]);
                }
                catch
                {

                }

                return result;
            }
            catch
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

}

