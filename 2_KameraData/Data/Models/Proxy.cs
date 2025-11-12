using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public sealed class Proxy
    {
        public int Id { get; set; } = 0;
        public string? Type { get; set; }
        public string? IP { get; set; }
        public int? Port { get; set; } = 0;
        public string? Login { get; set; }
        public string? Password { get; set; }
        public bool IsActive { get; set; }
    }
}
