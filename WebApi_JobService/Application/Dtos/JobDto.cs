using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_JobService.Application.Dtos
{
    public class JobDto
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int TelegramId { get; set; }

        public string? JobNumber { get; set; }

        public string? JobLink { get; set; }

        public string? Token { get; set; }

        public string? UpdateRequestStatus { get; set; }

        public string Status { get; set; } = "Pending";

        // Навигационные свойства и коллекции сюда не тянем,
        // MaskService они не нужны
    }

}
