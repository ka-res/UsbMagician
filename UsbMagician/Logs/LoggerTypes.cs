using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsbMagician.Logs
{
    partial class Logger
    {
        public enum ContextTypes
        {
            LoggerInfo,
            AssemblyName,
            RegistryInfo,
            EventStatus,
            DriveStatus,
            DirectoryStatus,
            FileStatus,
            PropertyInfo,
            ErrorMessage,
            General,
            Empty
        }
    }
}
