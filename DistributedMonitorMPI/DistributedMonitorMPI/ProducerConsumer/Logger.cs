using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.ProducerConsumer
{
    class Logger
    {
        public static void LogProducedVal(long clock, int rank, string val, int pos)
        {
            Console.WriteLine($"{clock}#{rank} put value[{val}] at position [{pos}]");
        }

        public static void LogConsumedVal(long clock, int rank, string val, int pos)
        {
            Console.WriteLine($"{clock}#{rank} consumed value[{val}] from position [{pos}]");
        }
    }
}
