using System;
using System.IO;
using System.Text;

namespace UsbMagician.Logs
{
    partial class Logger
    {
        private static readonly string LogFileName = $@"logger_{DateTime.Now.ToShortDateString()}";
        private static string _fullPath;

        public static void CheckLogFileExistance(string systemPath)
        {
            var logFileNameTmp = new StringBuilder(LogFileName.Replace(':', '_').Replace('.', '_'));
            logFileNameTmp.Append(".log");
            var fullPath = Path.Combine(systemPath, logFileNameTmp.ToString());

            if (string.IsNullOrEmpty(_fullPath))
            {
                _fullPath = fullPath;
            }

            if (!File.Exists(fullPath))
            {
                var fileStream = new FileStream(_fullPath, FileMode.Create);
                fileStream.Close();

                WriteLine(ContextTypes.LoggerInfo.ToString(), "Log file successfully created");
                WriteLine(ContextTypes.Empty.ToString());
            }
            else
            {
                WriteLine(ContextTypes.LoggerInfo.ToString(), "Log file already exists");
            }
        }

        public static void WriteLine(string logContext, string logMessage = null)
        {
            var logLine = logContext == ContextTypes.Empty.ToString()
                ? $"{DateTime.Now}"
                : $"{DateTime.Now}\t{logContext}:\t{logMessage}";

            using (var fileStream = new FileStream(_fullPath, FileMode.Append))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(logLine);
                }
            }

            Console.WriteLine(logLine);
        }
    }
}
