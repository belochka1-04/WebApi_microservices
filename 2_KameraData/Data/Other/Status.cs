using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Other
{
    public enum Status
    {
        Added = 0,
        Skip = 1,
        InWork = -1,
        Ready = 2,
        Error = 3
    }
}
