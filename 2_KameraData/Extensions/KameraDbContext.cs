using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData.Data
{
    public partial class KameraDbContext
    {
        protected string _connectionString;
        public KameraDbContext(string connString)
        {
            _connectionString = connString;
        }
    }
}
