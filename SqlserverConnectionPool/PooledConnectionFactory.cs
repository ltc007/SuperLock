using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseConnectionPool
{
  internal class PooledConnectionFactory:ConnectionFactoryBase
    {
        protected ConnectionStringBuilder Builder { get; private set; }

        private readonly object _syncObject = new object();
        /// <summary>
        /// 空闲的连接集合
        /// </summary>
        private readonly Queue<DbConnection> _idleConnections = new Queue<DbConnection>();
        /// <summary>
        /// 使用的连接集合
        /// </summary>
        private readonly List<DbConnection> _usedConnections = new List<DbConnection>();
        /// <summary>
        /// 无效的连接集合
        /// </summary>
        private readonly List<DbConnection> _invalidConnections = new List<DbConnection>();

        public PooledConnectionFactory(String connectionString, DbProviderFactory dbProviderFactory)
            : base(connectionString, dbProviderFactory)
        {

        }

        public int PoolSize
        {
            get
            {
                lock (_syncObject)
                    return _idleConnections.Count + _usedConnections.Count;
            }
        }

        /// <summary>
        /// 释放连接 
        /// </summary>
        /// <param name="dbConnection"></param>
        public override void Close(DbConnection dbConnection)
        {
            if (dbConnection != null)
            {
                if (IsAlive(dbConnection))
                {
                    if (_idleConnections.Count < Builder.MaximumPoolSize)
                    {
                        _idleConnections.Enqueue(dbConnection);
                    }
                }

                lock (_syncObject)
                {
                    _usedConnections.Remove(dbConnection);
                    _idleConnections.Enqueue(dbConnection);
                    Monitor.Pulse(_syncObject);
                }

            }
            else
            {
                throw new ArgumentNullException("connection");
            }
        }


        private bool IsAlive(DbConnection dbConnection)
        {
            if (dbConnection != null)
            {
                if (dbConnection.State == ConnectionState.Closed)
                    return false;
                else
                    return true;
            }
            else
            {
                throw new ArgumentNullException("connection");
            }
        }
        

        public override DbConnection Open()
        {
            DbConnection dbConnection;
            lock (_syncObject)
            {
                if (_idleConnections.Count()>0)
                {
                    dbConnection = _idleConnections.Dequeue();
                    _usedConnections.Add(dbConnection);
                    return dbConnection;
                }

                if (PoolSize >= Builder.MaximumPoolSize)
                {
                    if (!Monitor.Wait(_syncObject, Builder.ConnectionTimeout))
                        //等待时间过长
                        throw new ConnectionPoolException("Timeout expired. The timeout period elapsed prior to obtaining a connection from pool. This may have occured because all pooled connections were in use and max poolsize was reached.");

                    return Open();
                }
            }

            dbConnection = CreateDbConnection();
            lock (_syncObject)
                _usedConnections.Add(dbConnection);

            return dbConnection;
        }


        public override void Dispose()
        {
            lock (_syncObject)
            {
                foreach (var usedConnection in _usedConnections)
                    usedConnection.Dispose();

                foreach (var idleConnections in _idleConnections)
                    idleConnections.Dispose();

                foreach (var invalidConnection in _invalidConnections)
                    invalidConnection.Dispose();

                _usedConnections.Clear();
                _idleConnections.Clear();
                _invalidConnections.Clear();
            }
        }


    }
}
