using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    /// <summary>
    /// Обработчик обычных файлов
    /// </summary>
    public class OrdinaryFileHandler
    {
        public static void ReadFileContent(string pathToFile
            , Action<int, string>? lineReadingCallback
            , Action<Exception>? errorCallback)
        {
            try
            {
                using (StreamReader sr = new StreamReader(pathToFile))
                {
                    int lineIndex = 0;

                    while (!sr.EndOfStream)
                    {
                        lineReadingCallback?.Invoke(lineIndex, sr.ReadLine());
                        lineIndex++;
                    }
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(e);
            }

        }

        public static List<string> ReadFileContent(string pathToFile
            , Action<Exception>? errorCallback)
        {
            List<string> fileContent = new List<string>();

            try
            {
                using (StreamReader sr = new StreamReader(pathToFile))
                {
                    while (!sr.EndOfStream)
                    {
                        fileContent.Add(sr.ReadLine());
                    }
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(e);
            }

            return fileContent;
        }

        public static void WriteFileContent(string pathToFile
            , bool appending
            , List<string> content
            , Action<Exception>? errorCallback)
        {
            try
            {
                using (StreamWriter sr = new StreamWriter(pathToFile, appending))
                {
                    foreach (var line in content)
                    {
                        sr.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(e);
            }
        }

        public static void WriteFileContent(string pathToFile
            , bool appending
            , string content
            , Action<Exception>? errorCallback)
        {
            try
            {
                using (StreamWriter sr = new StreamWriter(pathToFile, appending))
                {
                    sr.Write(content);
                }
            }
            catch (Exception e)
            {
                errorCallback?.Invoke(e);
            }
        }

        public static string GetPathToFile(string pathToDirectory, string fileName)
        {
            if (!pathToDirectory.EndsWith("\\") && !pathToDirectory.EndsWith("/"))
            {
                pathToDirectory += "\\";
            }

            return $"{pathToDirectory}{fileName}";
        }
    }
}
