using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.Common
{
    public static class ConsoleLog
    {
        public static void WriteLog(string message)
        {
            WriteLog(message, LogLevel.Info);
        }

        public static void WriteLog(string message, LogLevel logLevel)
        {
            if (string.IsNullOrEmpty(message))
            {
                Console.WriteLine();
                return;
            }
            Console.WriteLine(DateTime.Now + " : " + message);

            switch (logLevel)
            {
                case LogLevel.Debug:
                    LogHelper.Debug(message);
                    break;
                case LogLevel.Info:
                    LogHelper.Info(message);
                    break;
                default:
                    LogHelper.Error(logLevel + "    " + message);
                    break;
            }
            LogHelper.Info(message);
        }

    }
}
