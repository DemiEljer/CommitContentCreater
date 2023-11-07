using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater
{
    public class UpdateHandler
    {
        public static void Update()
        {
            try
            {
                Process.Start($"{AppDomain.CurrentDomain.BaseDirectory}update.exe", "https://github.com/DemiEljer/CommitContentCreater/releases/latest/download/update.rar");
            }
            catch
            {
                Console.WriteLine("! Ошибка запуска утилиты обновления !");
            }

            return;
        }

    }
}
