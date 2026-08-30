using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models;

public partial class GoogleModelRequest
    {
        public int Id { get; set; }

        public string Request { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? LastCheckedAt { get; set; }

        /// <summary>
        /// Статус запроса (0 – новый, 1 – обработан и т.п.).
        /// </summary>
        public byte Status { get; set; }

        public string? WorkerId { get; set; }

        public DateTime? LeaseUntil { get; set; }

        public int AttemptCount { get; set; }

        public string? LastError { get; set; }
    }

