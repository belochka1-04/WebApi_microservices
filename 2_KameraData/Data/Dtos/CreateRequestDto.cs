using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class CreateRequestDto
    {
        public string RecognizedPartNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
