using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater.Handlers
{
    internal class LoggingHandler
    {
        public static void LogError(string message)
        {
            Console.WriteLine($"! Ошибка :: \"{message}\" !");
        }

        public static void LogTest(string message)
        {
            Console.WriteLine($"? ТЕСТ :: \"{message}\" ?");
        }
    }
}
