using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public class SharedStock
    {
        public int UserId { get; set; }           // кто видит
        public int StockId { get; set; }          // какой сток видит
        public int? GrantedByUserId { get; set; } // кто дал доступ (опционально)
        public DateTime CreatedAt { get; set; }   // дата создания

        // Навигационные свойства (если нужно)
        // public User User { get; set; }
        // public Stock Stock { get; set; }
        // public User GrantedByUser { get; set; }
    }

}
