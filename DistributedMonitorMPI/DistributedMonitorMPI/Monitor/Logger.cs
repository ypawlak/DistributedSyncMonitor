using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.Monitor
{
    static class Logger
    {
        public static void LogCSEntry(int procRank, long entryNo)
        {
            Console.WriteLine(string.Format("#{0} made {1} entry to CS", procRank, entryNo));
        }

        public static void LogCSExit(int procRank, long entryNo)
        {
            Console.WriteLine(string.Format("#{0} finished {1} entry to CS", procRank, entryNo));
        }

        public static void LogCSWait(int procRank, long entryNo, string varName)
        {
            Console.WriteLine(string.Format("#{0} finished {1} entry to CS and begun waiting in queue of conditional variable {2} entry to CS",
                procRank, entryNo, varName));
        }
        
    }
}
