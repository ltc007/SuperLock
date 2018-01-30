using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReaderWriterLockSlimExtend
{
    class Program
    {
        private static readonly Striped<ReaderWriterLockSlim> weakLock = Striped<ReaderWriterLockSlim>.lazyWeakLock(1024 * 10);
        static void Main(string[] args)
        {


            String key = "key";
            ReaderWriterLockSlim wrls = weakLock.get(key);

        }
    }
}
