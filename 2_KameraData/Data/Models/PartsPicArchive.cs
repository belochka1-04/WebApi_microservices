using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public class PartsPicArchive
    {
        public int Id { get; set; } // Уникальный идентификатор записи
        public int PartsAndReplacesId { get; set; } // Ссылка на запись в таблице parts_and_replaces
        public string LocalPath { get; set; } // Локальный путь к изображению
        public string Link { get; set; } // Ссылка на оригинальное изображение
        public int PartsSourcesId { get; set; } // ID сайта-источника
        public int AttemptCounter { get; set; } // Количество попыток подключения к сайту
        public DateTime CreatedAt { get; set; } // Дата создания записи
        public DateTime UpdatedAt { get; set; } // Дата последнего обновления

        // Конструктор по умолчанию
        public PartsPicArchive()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            AttemptCounter = 0; // Инициализация счетчика попыток
        }
    }
}
