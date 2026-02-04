using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class UserStockDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StockId { get; set; }
        public int? PartsAndReplacesId { get; set; }
        public int Quantity { get; set; }

        // Вложенные данные склада
        public StockDto? Stock { get; set; }
    }
}
