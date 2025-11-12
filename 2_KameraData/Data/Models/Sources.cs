using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public sealed class Sources
    {
        /// <summary>
        /// ID источника
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// [source_name] Имя источника
        /// </summary>
        public string SourceName { get; set; } = string.Empty;

        public int Confidence { get; set; } = 0;

        /// <summary>
        /// [data_types] Массив int, который хранит все возможные типы документации
        /// </summary>
        public string DataTypes { get; set; } = string.Empty;

        /// <summary>
        /// [folder_path] Путь к папке с документацией
        /// </summary>
        public string? FolderPath { get; set; } = string.Empty;
    }
}
