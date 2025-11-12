using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Settings
{
    public sealed class OtherSettings
    {
        public int MaxThreads { get; set; }
        public string PathWithPdf { get; set; } = string.Empty;

        public string WebApi { get; set; }

        public string LoggsFolder { get; set; }

        public string UsersFolders { get; set; }

        public string ScriptName { get; set; } 
    }
}
