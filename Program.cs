using System.Linq;

bool cleanFilesFlag = true;
string projectDirrectory = "./";
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

FileHandling.GenerateCommitFile(projectDirrectory, resultLines);

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
            if (!string.IsNullOrEmpty(resultString))
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

        return resultString;
    }

    public static void GenerateCommitFile(string path, List<string> lines)
    {
        string pathToFile = path;

        if (!path.EndsWith("\\"))
        {
            pathToFile += "\\";
        }
        pathToFile += "commit.txt";

        using (StreamWriter sw = new StreamWriter(pathToFile))
        {
            sw.WriteLine("v a.b.c");
            sw.WriteLine("");

            foreach (var line in lines)
            {
                sw.WriteLine(line);
            }
        }

        Console.WriteLine($"Был сгенерирован файл коммита по пути: {pathToFile}");
    }
}
