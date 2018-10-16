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

        public static void LogRequestingWithReceivedAcks(int rank, int receivedAcks)
        {
            Console.WriteLine($"#{rank} requesting, obtained: {receivedAcks} acks");
        }

        public static void LogWait(int rank, long entryNo)
        {
            Console.WriteLine($"#{rank} called Wait and finished {entryNo} entry to CS");
        }

        public static void LogPreSignal(int rank, int awProcess, long entryNo)
        {
            Console.WriteLine($"#{rank} called Signal for awakening process {awProcess} in {entryNo} entry to CS");
        }

        public static void LogAfterSignalInCS(int rank, long entryNo)
        {
            Console.WriteLine($"#{rank} made {entryNo} entry to CS after Signal ");
        }
    }
}
