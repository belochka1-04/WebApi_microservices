using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public class GoogleSerpRaw
    {
        public int Id { get; set; }

        public int GoogleModelRequestId { get; set; }

        public string QueryText { get; set; } = null!;

        public int GooglePage { get; set; }

        public string? Title { get; set; }

        public string? Snippet { get; set; }

        public string Url { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}

