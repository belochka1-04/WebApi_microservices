using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public class PartSource
    {
        public int Id { get; set; } // Уникальный идентификатор источника
        public string SourceName { get; set; } // Название источника
        public string DataType { get; set; } // Тип данных (например, "picture", "partname")
        public string Link { get; set; } // Шаблон ссылки для формирования запросов
        public string Confidence { get; set; } // Шаблон ссылки для формирования запросов
        public DateTime CreatedAt { get; set; } // Дата создания записи
        public DateTime UpdatedAt { get; set; } // Дата последнего обновления записи
        public bool Status { get; set; } // Статус источника (bit в базе данных)


        // Конструктор по умолчанию
        public PartSource()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        // Конструктор с параметрами
        public PartSource(string sourceName, string dataType, string link)
        {
            SourceName = sourceName;
            DataType = dataType;
            Link = link;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
    }
}
