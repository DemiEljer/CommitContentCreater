using CommitContentCreater.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater.Handlers
{
    internal class ConfigFileHandler
    {
        public static string[] ParseConfigFile(string fileContent, Action<string>? parsingMessageCallback)
        {
            ConfigFileModel model = new ConfigFileModel();

            // Удаление строк комментариев
            fileContent = string.Join("", fileContent.Split("#").Where((e, i) => (i % 2) != 1).Where(e => !string.IsNullOrEmpty(e)));
      
            var configFileLines = fileContent.Replace("\r", "").Replace("\n", "").Split(";").Where(e => !string.IsNullOrEmpty(e));

            model.ProjectPath = ".";

            foreach (var configFileLine in configFileLines)
            {
                var configFileLineElements = configFileLine.Split(":").ToArray();

                if (configFileLineElements.Length > 2)
                {
                    parsingMessageCallback?.Invoke($"Ошибка конфигурационного файла. Строка имеет неверный формат :: \"{configFileLine}\"");
                    continue;
                }

                var paramName = configFileLineElements[0].Trim();

                if (paramName == ConfigFileModel.FileExtentionsKeyWord)
                {
                    if (configFileLineElements.Length != 2)
                    {
                        parsingMessageCallback?.Invoke($"Ошибка конфигурационного файла. Строка имеет неверный формат :: \"{configFileLine}\"");
                    }
                    else
                    {
                        var extentions = configFileLineElements[1].Split(",").Select(e => e.Trim()).Select(e => e.Replace("\"", "").Replace("'", "")).Where(e => !string.IsNullOrEmpty(e));

                        model.FileExtentions.AddRange(extentions);
                    }
                }
                else if (paramName == ConfigFileModel.FilesKeyWord)
                {
                    if (configFileLineElements.Length != 2)
                    {
                        parsingMessageCallback?.Invoke($"Ошибка конфигурационного файла. Строка имеет неверный формат :: \"{configFileLine}\"");
                    }
                    else
                    {
                        var files = configFileLineElements[1].Split(",").Select(e => e.Trim()).Select(e => e.Replace("\"", "").Replace("'", "")).Where(e => !string.IsNullOrEmpty(e));

                        model.Files.AddRange(files);
                    }
                }
                else if (paramName == ConfigFileModel.ProjectPathKeyWord)
                {
                    if (configFileLineElements.Length != 2)
                    {
                        parsingMessageCallback?.Invoke($"Ошибка конфигурационного файла. Строка имеет неверный формат :: \"{configFileLine}\"");
                    }
                    else
                    {
                        var pathToProjet = configFileLineElements[1].Trim();

                        if (string.IsNullOrEmpty(pathToProjet))
                        {
                            pathToProjet = ".";
                        }
                        model.ProjectPath = pathToProjet;
                    }
                }
                else if (paramName == ConfigFileModel.ClearCommitsKeyWord)
                {
                    if (configFileLineElements.Length != 1)
                    {
                        parsingMessageCallback?.Invoke($"Ошибка конфигурационного файла. Строка имеет неверный формат :: \"{configFileLine}\"");
                    }
                    else
                    {
                        model.ClearCommitLines = true;
                    }
                }
                else if (paramName == ConfigFileModel.ShowTracingKeyWord)
                {
                    if (configFileLineElements.Length != 1)
                    {
                        parsingMessageCallback?.Invoke($"Ошибка конфигурационного файла. Строка имеет неверный формат :: \"{configFileLine}\"");
                    }
                    else
                    {
                        model.ShowTracing = true;
                    }
                }
                else
                {
                    parsingMessageCallback?.Invoke($"Ошибка конфигурационного файла. Неверное имя параметра :: \"{paramName}\"");
                }
            }

            return GetArgsFromConfigFileModel(model);
        }

        public static string[] GetArgsFromConfigFileModel(ConfigFileModel model)
        {
            List<string> args = new List<string>();

            if (!string.IsNullOrEmpty(model.ProjectPath))
            {
                args.Add(model.ProjectPath);
            }

            if (model.ClearCommitLines)
            {
                args.Add("-c");
            }

            if (model.ShowTracing)
            {
                args.Add("-t");
            }

            foreach (var file in model.Files)
            {
                args.Add("-f");
                args.Add(file);
            }

            foreach (var extention in model.FileExtentions)
            {
                args.Add("-e");
                args.Add(extention);
            }

            return args.ToArray();
        }
    }
}
