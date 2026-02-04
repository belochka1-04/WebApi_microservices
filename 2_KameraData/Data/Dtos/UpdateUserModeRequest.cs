using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class UpdateUserModeRequest
    {
        public byte Mode { get; set; } // 0 = repair, 1 = warehouse и т.д.
    }
}
