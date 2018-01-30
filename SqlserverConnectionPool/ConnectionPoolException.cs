using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectionPool
{
   public class ConnectionPoolException : Exception
    {
        public ConnectionPoolException(string message, Exception innerException):base(message,innerException){ }

        public ConnectionPoolException(string message):base(message){ }

    }
}
