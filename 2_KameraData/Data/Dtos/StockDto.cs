using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class StockDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Location { get; set; }

        public string? Description { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? ContactPerson { get; set; }

        public string? Email { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Если нужно отображать принадлежность (свой/чужой склад)
        public bool IsOwnStock { get; set; }

        // Если есть связь с пользователем через UserStock
        public int? UserStockId { get; set; }

        public int Quantity { get; set; }
    }

}
