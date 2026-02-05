using KameraData.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data.Dtos
{
    public class TipsResponse
    {
        public TipsVideo? Video { get; set; }
        public TipsLink? Link { get; set; }
    }
}
