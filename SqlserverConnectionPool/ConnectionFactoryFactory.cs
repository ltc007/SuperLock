using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectionPool
{
    class ConnectionFactoryFactory
    {
        private static readonly Dictionary<string, IConnectionFactory> Factorys = new Dictionary<string, IConnectionFactory>();
        private static readonly object SyncObject = new object();

        public static int PoolCount
        {
            get
            {
                lock (SyncObject)
                    return Factorys.Count;
            }
        }

        public static void Shutdown()
        {
            lock (SyncObject)
            {
                foreach (var pool in Factorys.Values)
                    pool.Dispose();

                Factorys.Clear();
            }
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static DbConnection GetConnection(string connectionString,DbProviderFactory dbProviderFactory)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");

            IConnectionFactory pool;

            lock (SyncObject)
            {
                if (!Factorys.TryGetValue(connectionString, out pool))
                    Factorys.Add(connectionString, pool = CreateFactory(connectionString, dbProviderFactory));
            }

            return pool.Open();
        }

        private static IConnectionFactory CreateFactory(string connectionString, DbProviderFactory dbProviderFactory)
        {
            var builder = new ConnectionStringBuilder();

            if (builder.Pooled)
                return new PooledConnectionFactory(connectionString, dbProviderFactory);

            return new SimpleConnectionFactory(connectionString, dbProviderFactory);
        }

    }
}
