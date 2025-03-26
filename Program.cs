using CommitContentCreater;
using CommitContentCreater.Handlers;
using CommitContentCreater.Models;
using CommitContentCreater.Properties;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;


internal class Program
{
    private static void Main(string[] args)
    {
        string applicationVersion = "v1.3.1";

        bool cleanFilesFlag = false;
        string projectDirrectory = "";
        string gitHistoryExtractionPath = "./log.txt";
        List<string> fileExtentions = new List<string>();
        List<string> filePathes = new List<string>();

        bool extentionReading = false;
        bool helpShow = false;
        bool findVersion = false;
        bool fromGitCommitFileExtraction = false;
        bool fromGitCommitFileExtractionCommand = false;
        bool filePathReading = false;
        bool showFoundFiles = false;
        bool generateXMLHistoryFile = false;

        var ArgsHandlingDelegate = (string[] args) =>
        {
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].Replace("\"", "").Replace("'", "");

                try
                {
                    if (args[i] == "-c")
                    {
                        cleanFilesFlag = true;
                        extentionReading = false;
                        fromGitCommitFileExtraction = false;
                        filePathReading = false;
                    }
                    else if (args[i] == "-e")
                    {
                        extentionReading = true;
                        fromGitCommitFileExtraction = false;
                        filePathReading = false;
                    }
                    else if (args[i] == "-h")
                    {
                        helpShow = true;
                        extentionReading = false;
                        fromGitCommitFileExtraction = false;
                        filePathReading = false;
                    }
                    else if (args[i] == "-v")
                    {
                        findVersion = true;
                        extentionReading = false;
                        fromGitCommitFileExtraction = false;
                        filePathReading = false;
                    }
                    else if (args[i] == "-g")
                    {
                        fromGitCommitFileExtractionCommand = true;
                        fromGitCommitFileExtraction = true;
                        extentionReading = false;
                        filePathReading = false;
                    }
                    else if (args[i] == "-gx")
                    {
                        generateXMLHistoryFile = true;
                        fromGitCommitFileExtraction = false;
                        extentionReading = false;
                        filePathReading = false;
                    }
                    else if (args[i] == "-f")
                    {
                        filePathReading = true;
                        extentionReading = false;
                        fromGitCommitFileExtraction = false;
                    }
                    else if (args[i] == "-t")
                    {
                        showFoundFiles = true;
                        extentionReading = false;
                        fromGitCommitFileExtraction = false;
                        filePathReading = false;
                    }
                    else if (args[i] == "-u")
                    {
                        //UpdateHandler.Update();
                        Console.WriteLine("Внимение. Обновление версии ПО пока что не доступно!");

                        return;
                    }
                    else if (!string.IsNullOrEmpty(args[i]))
                    {
                        if (extentionReading)
                        {
                            fileExtentions.Add(args[i]);
                        }
                        else if (fromGitCommitFileExtraction)
                        {
                            gitHistoryExtractionPath = args[i];
                        }
                        else if (filePathReading)
                        {
                            filePathes.Add(args[i]);
                        }
                        else
                        {
                            projectDirrectory = args[i];
                        }

                        extentionReading = false;
                        fromGitCommitFileExtraction = false;
                        filePathReading = false;
                    }
                    else
                    {
                        LoggingHandler.LogError("Ошибка чтения аргумента");
                        Console.WriteLine("Посмотреть формат аргуменов утилиты можно с помощью параметра -h.");
                    }
                }
                catch
                {

                }
            }
        };

        if (args.Length == 0)
        {
            try
            {
                using (var fr = new StreamReader("./commit.gen"))
                {
                    args = ConfigFileHandler.ParseConfigFile(fr.ReadToEnd(), (message) =>
                    {
                        Console.WriteLine(message);
                    });

                    //Console.WriteLine(string.Join(",", args));
                }
            }
            catch
            {
                Console.WriteLine("Файл конфигурации (commit.gen) не был найден или произошла ошибка чтения.");
            }
        }

        // Обработка аргументов
        ArgsHandlingDelegate(args);

        // Нормализация путей
        {
            var pathNormalizer = (string path) => path.Replace("\\", "/").Trim();

            projectDirrectory = pathNormalizer(projectDirrectory);
            gitHistoryExtractionPath = pathNormalizer(gitHistoryExtractionPath);
            filePathes = filePathes.Select(p => pathNormalizer(p)).ToList();
        }

        CommitContentCreater.VersionModel currentVersion = CommitFileHandler.FindVersionInHistoryFile(projectDirrectory);

        if (findVersion)
        {
            Console.WriteLine(currentVersion.ToString());

            return;
        }

        Console.WriteLine($"CommitContentCreater {applicationVersion}");
        Console.WriteLine("==========================================");

        if (helpShow)
        {
            using (StreamReader sr = new StreamReader(new MemoryStream(Resources.help)))
            {
                Console.WriteLine(sr.ReadToEnd());
            }
        }

        if (generateXMLHistoryFile)
        {
            XMLHistoryFileHandler.GenerateHistoryFile(projectDirrectory);
        }
        else if (fromGitCommitFileExtractionCommand)
        {
            CommitHistoryExtracter.HistoryFileExtraction(gitHistoryExtractionPath);
        }

        if (helpShow || 
            findVersion || 
            fromGitCommitFileExtractionCommand ||
            generateXMLHistoryFile
            )
        {
            return;
        }

        CommitModel commitModel = new CommitModel();
        commitModel.Date =  DateTime.Now;
        VersionModel prevVersion = currentVersion.Clone();

        int foundFilesCount = 0;

        try
        {
            foreach (var filePath in Directory.GetFiles(projectDirrectory, "*.*", SearchOption.AllDirectories))
            {
                var _filePath = filePath.Replace("\\", "/");

                string extention = filePath.Split('.').Last();

                if (filePathes.Find(e => filePath.EndsWith(e)) != null 
                    || fileExtentions.Contains(extention))
                {
                    CommitFileHandler.FindCommitLines(commitModel, filePath, cleanFilesFlag);

                    foundFilesCount++;
                    if (showFoundFiles)
                    {
                        Console.WriteLine($"Был обнаружен и обработан файл: {filePath}");
                    }
                }
            }
        }
        catch
        {
            LoggingHandler.LogError("Ошибка доступа к директории проекта");
        }

        // Модификация версии
        if (commitModel.Version.IsEmpty)
        {
            commitModel.IsNewVersion = false;
        }
        commitModel.Version = currentVersion + commitModel.Version;

        if (commitModel.Lines.Length != 0)
        {
            CommitFileHandler.GenerateCommitFile(projectDirrectory, commitModel);
            CommitFileHandler.AppendHistoryFile(projectDirrectory, commitModel);

            Console.WriteLine($"Было обнаружено и обработано файлов: {foundFilesCount}.");
        }
        else
        {
            LoggingHandler.LogError("Не было найдено ни одной строки коммита");
        }
    }
}



