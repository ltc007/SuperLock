using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectionPool
{
    internal abstract class ConnectionFactoryBase : IConnectionFactory
    {
        public string ConnectionString { get; set; }
        public DbProviderFactory DbProviderFactory { get; set; }


        protected ConnectionFactoryBase(string connectionString, DbProviderFactory dbProviderFactory)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            ConnectionString = connectionString;
            DbProviderFactory = dbProviderFactory;
        }

        public virtual void Cleanup()
        {
        }

        public abstract void  Close(DbConnection dbConnection);

        protected DbConnection CreateDbConnection()
        {
            DbConnection dbConnection = DbProviderFactory.CreateConnection();
            dbConnection.ConnectionString = ConnectionString;
            return dbConnection;
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }

        public abstract DbConnection Open();
    }
}
