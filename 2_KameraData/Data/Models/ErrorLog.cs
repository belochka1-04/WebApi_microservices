using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public class ErrorLog
    {
        public int id { get; set; }
        public int? user_id { get; set; }
        public DateTime? timestamp { get; set; }
        public string? error_type { get; set; }
        public string? details { get; set; }
        public string? action_taken { get; set; }
        public string? screen_name { get; set; }
        public string? request_data { get; set; }
        public int? job_id { get; set; }
        public string? status { get; set; }
        public string script_name { get; set; } = null!; // Non-nullable, initialize!
        public string error_message { get; set; } = null!; // Non-nullable, initialize!
        public DateTime? error_time { get; set; }
        public int? stock_id { get; set; }
        public string? additional_data1 { get; set; }
        public string? additional_data2 { get; set; }
        public string? additional_data3 { get; set; }
        public string? additional_data4 { get; set; }
        public string? additional_data5 { get; set; }
    }
}
