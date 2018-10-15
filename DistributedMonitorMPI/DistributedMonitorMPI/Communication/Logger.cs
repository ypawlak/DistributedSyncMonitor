using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.Communication
{
    static class Logger
    {
        public static void LogSent(int fromRank, int toRank, int tag)
        {
            Console.WriteLine(string.Format("#{0} SENT {1} TO {2}", fromRank, tag, toRank));
        }

        public static void LogReceived(int receiverRank, int senderRank, int tag)
        {
            Console.WriteLine(string.Format("#{0} RECEIVED {1} FROM {2}", receiverRank, tag, senderRank));
        }
    }
}
