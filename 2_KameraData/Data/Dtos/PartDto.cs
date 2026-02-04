using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class PartDto
    {
        public int Id { get; set; }
        public string? MainPartNumber { get; set; }
        public string? Status { get; set; }
        public int? PhotoStatus { get; set; }
        public DateTime? DateUpdate { get; set; }

        // Вложенные остатки (агрегируются в боте из UserService)
        public List<UserStockDto> UserStocks { get; set; } = new();
    }
}
