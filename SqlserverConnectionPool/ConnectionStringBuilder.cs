using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectionPool
{
    class ConnectionStringBuilder
    {
        /// <summary>
        /// </summary>
        public const int DefaultMaximumPoolSize = 100;

        /// <summary>
        /// </summary>
        public const int DefaultMinimumPoolSize = 0;


        public TimeSpan ConnectionTimeout { get; set; }

        public int MaximumPoolSize { get; set; }

        public bool Pooled { get; set; }

    }
}
