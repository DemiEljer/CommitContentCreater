﻿using CommitContentCreater;
using CommitContentCreater.Models;
using System;
using System.Linq;

internal class Program
{
    private static void Main(string[] args)
    {
        string applicationVersion = "v1.1.0";

        bool cleanFilesFlag = false;
        string projectDirrectory = "D:\\Work\\Lipgart\\TMS\\Projects\\PAZ_Vector_PT01_Project\\VCU_PAZ_Vector_Electro_PT101";//".\\";
        string gitHistoryExtractionPath = ".\\log.txt";
        List<string> fileExtentions = new List<string>();

        fileExtentions.Add("h");
        fileExtentions.Add("c");

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
            Console.WriteLine("-g <path>         : Сгенерировать файл истории из лога git;");
            Console.WriteLine("-v                : Вывести текущую версию проекта;");
            Console.WriteLine("-h                : Выводит справочную ифнормацию по утилите;");
            Console.WriteLine("-c                : Флаг очистка проекта от строк с описанием коммита;");
            Console.WriteLine("-e <extention>    : Указание допустимое расширения файла для поиска описания коммита (-e h -e c);");
            Console.WriteLine("<Path>            : Путь к директории указывается просто как строка.");
            Console.WriteLine("Аспекты нотации оформления:");
            Console.WriteLine("//~ [Comment]     : Строка комментария коммита (одиночная)");
            Console.WriteLine("/*~ [Comments] */ : Строка комментария коммита (многосточная)");
            Console.WriteLine("//~ va.b.c        : Указание дельты новой версии (//~ v0.0.1 : v1.0.0 -> v1.0.1)");
            Console.WriteLine("-[-][-]...        : Управление табуляцией (вложенностью)");
            Console.WriteLine("Сокращения и модификаторы:");
            Console.WriteLine("#!                : Комментарий-заголовок (без -)");
            Console.WriteLine("#[Number]         : Позиция комментария в коммите");
            Console.WriteLine("#f                : [FIX]");
            Console.WriteLine("#m                : [MODIFY]");
            Console.WriteLine("#e                : [EVENT]");
            Console.WriteLine("#u                : [UPDATE]");
            Console.WriteLine("#a                : [ADD]");
            Console.WriteLine("#i                : [INFORMATION]");
            Console.WriteLine("#n                : [NOTE]");
            Console.WriteLine("#c                : [CORRECTION]");
            Console.WriteLine("#im               : [IMPROVEMENT]");
        }

        if (fromGitCommitFileExtraction)
        {
            CommitHistoryExtracter.HistoryFileExtraction(gitHistoryExtractionPath);
        }

        CommitContentCreater.VersionModel currentVersion = CommitFileHander.FindVersionInHistoryFile(projectDirrectory);

        if (findVersion)
        {
            Console.WriteLine(currentVersion.ToString());
        }

        if (helpShow || findVersion || fromGitCommitFileExtraction)
        {
            return;
        }

        CommitModel commitModel = new CommitModel();
        VersionModel prevVersion = currentVersion.Clone();

        Console.WriteLine($"CommitContentCreater {applicationVersion}");
        Console.WriteLine("==========================================");

        try
        {
            foreach (var filePath in Directory.GetFiles(projectDirrectory, "*.*", SearchOption.AllDirectories))
            {
                string extention = filePath.Split('.').Last();

                if (fileExtentions.Contains(extention))
                {
                    CommitFileHander.FindCommitLines(commitModel, filePath, cleanFilesFlag);
                }
            }
        }
        catch
        {
            Console.WriteLine("! Ошибка доступа к директории проекта !");
        }

        // Модификация версии
        if (commitModel.Version.IsEmpty)
        {
            commitModel.IsNewVersion = false;
        }
        commitModel.Version = currentVersion + commitModel.Version;

        if (commitModel.Lines.Length != 0)
        {
            CommitFileHander.GenerateCommitFile(projectDirrectory, commitModel);
            CommitFileHander.AppendHistoryFile(projectDirrectory, commitModel);
        }
        else
        {
            Console.WriteLine("! Не было найдено ни одной строки коммита !");
        }
    }
}


