using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.Monitor
{
    static class Logger
    {
        public static void LogCSEntry(int procRank, long entryNo, long clock)
        {
            Console.WriteLine($"{clock}#{procRank} made {entryNo} entry to CS");
        }

        public static void LogCSExit(int procRank, long entryNo, long clock)
        {
            Console.WriteLine($"{clock}#{procRank} finished {entryNo} entry to CS");
        }

        public static void LogCSWait(int procRank, long entryNo, string varName, long clock)
        {
            Console.WriteLine($"{clock}#{procRank} finished {entryNo} entry to CS and begun waiting in queue of conditional variable {varName} entry to CS");
        }

        public static void LogRequestingWithReceivedAcks(int rank, int receivedAcks, long clock)
        {
            //Console.WriteLine($"{clock}#{rank} requesting, obtained: {receivedAcks} acks");
        }

        public static void LogWait(int rank, long entryNo, long clock)
        {
            Console.WriteLine($"{clock}#{rank} called Wait and finished {entryNo} entry to CS");
        }

        public static void LogPreSignal(int rank, int awProcess, long entryNo, long clock)
        {
            Console.WriteLine($"{clock}#{rank} called Signal for awakening process {awProcess} in {entryNo} entry to CS");
        }

        public static void LogAfterSignalInCS(int rank, long entryNo, long clock)
        {
            Console.WriteLine($"{clock}#{rank} made {entryNo} entry to CS after Signal ");
        }
    }
}
