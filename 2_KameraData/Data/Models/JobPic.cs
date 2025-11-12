using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Models
{
    public partial class JobPic
    {
        public int Id { get; set; }
        public string? FolderName { get; set; }
        public string? FileName { get; set; }
        public string? OcrText { get; set; }
        public int IsRating { get; set; }
        public decimal CompletionCosts { get; set; }
        public int ImageToken { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }

    }
}
