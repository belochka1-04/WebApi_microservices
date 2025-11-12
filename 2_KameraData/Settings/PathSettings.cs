using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Settings
{
    public sealed class PathSettings
    {
        public string imagePath { get; set; } = string.Empty;
        public string ProcessedFilesPath { get; set; } = string.Empty;
        public string LogFilePath { get; set; } = string.Empty;
        public string LogFolder { get; set; } = string.Empty;
        public string ProcessedFolder { get; set; } = string.Empty;
        public string tessDataPath { get; set; } = string.Empty;
    }
}
