using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public class FinderModel
    {
        public int job_id { get; set; }
        public string model_name { get; set; }
        public string brand { get; set; }
        public string finder_text { get; set; } = string.Empty;
        public Model model { get; set; }
        public int state { get; set; }
        public int count { get; set; }
        public bool useTrim { get; set; }
        public int modelsCount { get; set; }
        public int step { get; set; }
        public bool newerSetState { get; set; }

    }
}
