using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public sealed class UpdateUserRequest
    {
        public string? FullName { get; set; }
        public bool? IsActive { get; set; }
    }
}
