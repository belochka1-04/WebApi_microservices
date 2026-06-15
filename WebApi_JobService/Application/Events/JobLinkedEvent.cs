using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_JobService.Application.Events
{
    public record JobLinkedEvent
    {
        public int JobId { get; init; }
        public int UserId { get; init; }
        public int TelegramId { get; init; }
    }
}
