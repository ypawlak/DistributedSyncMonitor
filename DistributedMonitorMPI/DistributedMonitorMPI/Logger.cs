using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI
{
    class Logger
    {
        public static void LogSent(int fromRank, int toRank, string tag)
        {
            Console.WriteLine(string.Format("#{0} SENT {1} TO {2}", fromRank, tag, toRank));
        }
    }
}
