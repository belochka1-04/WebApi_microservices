using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Events
{
    public record JobCreatedEvent
    {
        public int JobId { get; init; }
        public int TelegramId { get; init; }
    }
}