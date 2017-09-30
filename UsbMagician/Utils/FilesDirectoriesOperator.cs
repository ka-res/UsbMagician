using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UsbMagician.Logs;

namespace UsbMagician.Utils
{
    class FilesDirectoriesOperator
    {
        public static void GetFiles(string sourceDirectory, string targetPath, int directoryLevel)
        {
            var fileTypes = new[]
            {
                "ppt", "pptx", "doc", "docx", "pdf", "png", "jpg", "jpeg", "xls", "xlsx", "mp3", "wav",
                "txt", "log", "ogg", "mkv", "bmp", "avi", "mp4", "mpg4", "mpeg4", "rtf"
            };

            foreach (var fileType in fileTypes)
            {
                foreach (var sourceFile in Directory.GetFiles(sourceDirectory, $"*.{fileType}", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        var sourceFilePathAndName = Path.Combine(sourceDirectory, Path.GetFileName(sourceFile));
                        var targetFilePathAndName = Path.Combine(targetPath, Path.GetFileName(sourceFile));

                        File.Copy(sourceFile, targetFilePathAndName, false);
                        Logger.WriteLine(Logger.ContextTypes.FileStatus.ToString(), $"Source file: {sourceFilePathAndName}\n\t " +
                                                                                    $"-> target destination: {targetFilePathAndName}");
                    }
                    catch (Exception error)
                    {
                        Logger.WriteLine(Logger.ContextTypes.ErrorMessage.ToString(), error.Message);
                    }

                    Logger.WriteLine(Logger.ContextTypes.DirectoryStatus.ToString(), $"Copying files finished at level {directoryLevel} ({sourceDirectory})");
                }
            }

            var subDirectories = Directory.GetDirectories(sourceDirectory);

            if (subDirectories.Length <= 0) return;

            directoryLevel--;

            foreach (var subDirectory in subDirectories)
            {
                try
                {
                    GetFiles(subDirectory, targetPath, directoryLevel);
                }
                catch (Exception error)
                {
                    Logger.WriteLine(Logger.ContextTypes.ErrorMessage.ToString(), error.Message);
                }
            }
        }
    }
}
