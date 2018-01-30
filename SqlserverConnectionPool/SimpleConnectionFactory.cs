using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectionPool
{
  internal  class SimpleConnectionFactory: ConnectionFactoryBase
    {
        public SimpleConnectionFactory(String connectionString, DbProviderFactory dbProviderFactory) :
            base(connectionString, dbProviderFactory)
        {

        }

        public override DbConnection Open()
        {
            return base.CreateDbConnection();
        }

        public override void Close(DbConnection dbConnection)
        {
            if (dbConnection!=null)
            {
                dbConnection.Close();
            }
        }
    }
}
