using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public class ModelLinkHistory
    {
        public int Id { get; set; }

        public int ModelId { get; set; }

        public string OldLink { get; set; } = null!;

        public DateTime ChangedAt { get; set; }

        public string? Source { get; set; }

        // Навигационное свойство к модели
        public ModelTb Model { get; set; } = null!;
    }
}

