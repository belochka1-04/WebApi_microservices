using KameraData.Data;
using KameraData.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KameraData
{
    public class DataProvider
    {
        private KameraDbContext _db;
        public DataProvider(string connectionString)
        {
            _db = new KameraDbContext(connectionString);
        }

        public int CheckAppState(string appName)
        {
            var app = _db.Applications.FirstOrDefault(x => x.Name == appName);
            if (app != null)
            {
                app.LastRunDate = DateTime.Now;
                _db.SaveChanges();
                return app.Status ?? 0;
            }
            else
                return 0;
        }

    }
}
