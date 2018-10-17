using DistributedMonitorMPI.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.Communication
{
    static class Logger
    {
        public static void LogSent(int fromRank, int toRank, int tag, long clock)
        {
            Console.WriteLine($"{clock}#{fromRank} SENT {Tags.TagsDict[tag]} TO {toRank}");
        }

        public static void LogReceived(int receiverRank, int senderRank, int tag, long clock)
        {
            Console.WriteLine($"{clock}#{receiverRank} RECEIVED {Tags.TagsDict[tag]} FROM {senderRank}");
        }
    }
}
