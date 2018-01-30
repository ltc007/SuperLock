using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectionPool
{
   public  interface IConnectionFactory : IDisposable
    {
        DbConnection Open();

        void Close(DbConnection dbConnection);

        String ConnectionString { get; }

        void Cleanup();
    }
}
