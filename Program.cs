using CommitContentCreater;
using System;
using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        string applicationVersion = "v1.0.0";

        bool cleanFilesFlag = false;
        string projectDirrectory = ".\\";
        string gitHistoryExtractionPath = ".\\log.txt";
        List<string> fileExtentions = new List<string>();

        bool extentionReading = false;
        bool helpShow = false;
        bool findVersion = false;
        bool fromGitCommitFileExtraction = false;
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
                else if (args[i] == "-v")
                {
                    findVersion = true;
                }
                else if (args[i] == "-g")
                {
                    fromGitCommitFileExtraction = true;
                }
                else if (!string.IsNullOrEmpty(args[i]))
                {
                    if (extentionReading)
                    {
                        fileExtentions.Add(args[i]);
                        extentionReading = false;
                    }
                    else if (fromGitCommitFileExtraction)
                    {
                        gitHistoryExtractionPath = args[i];
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
            Console.WriteLine($"CommitContentCreater {applicationVersion}");
            Console.WriteLine("==========================================");
            Console.WriteLine("Описание аргументов:");
            Console.WriteLine("-g <path>      : Сгенерировать файл истории из лога git;");
            Console.WriteLine("-v             : Вывести текущую версию проекта;");
            Console.WriteLine("-h             : Выводит справочную ифнормацию по утилите;");
            Console.WriteLine("-c             : Флаг очистка проекта от строк с описанием коммита;");
            Console.WriteLine("-e <extention> : Указание допустимое расширения файла для поиска описания коммита (-e h -e c);");
            Console.WriteLine("<Path>         : Путь к директории указывается просто как строка.");
        }

        if (fromGitCommitFileExtraction)
        {
            CommitHistoryExtracter.HistoryFileExtraction(gitHistoryExtractionPath);
        }

        CommitContentCreater.Version currentVersion = FileHandling.FindVersionInHistoryFile(projectDirrectory);

        if (findVersion)
        {
            Console.WriteLine(currentVersion.ToString());
        }

        if (helpShow || findVersion || fromGitCommitFileExtraction)
        {
            return;
        }

        List<string> resultLines = new List<string>();
        CommitContentCreater.Version prevVersion = currentVersion.Clone();

        Console.WriteLine($"CommitContentCreater {applicationVersion}");
        Console.WriteLine("==========================================");

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

        // Поиск модификатора версии в файлах
        CommitContentCreater.Version versionModifier = new CommitContentCreater.Version();
        for (int i = 0; i < resultLines.Count;)
        {
            var _versionModifier = CommitContentCreater.Version.ParseFromString(resultLines[i]);

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
            FileHandling.AppendHistoryFile(projectDirrectory, resultLines, currentVersion, currentVersion.Compare(prevVersion) != 0);
        }
        else
        {
            Console.WriteLine("! Не было найдено ни одной строки коммита !");
        }
    }
}


