using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class CreateDocumentQaDto
    {
        public int AnalysisId { get; set; }
        public string Question { get; set; } = null!;
    }
}
