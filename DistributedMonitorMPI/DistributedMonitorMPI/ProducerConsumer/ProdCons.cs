using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DistributedMonitorMPI.Monitor;
using DistributedMonitorMPI.Communication;

namespace DistributedMonitorMPI.ProducerConsumer
{
    public class ProdCons : Monitor.Monitor<ProdConsInternals>
    {
        public ProdCons(MpiBroker communicator, long bufferSize) : base(communicator)
        {
            Internals = new ProdConsInternals()
            {
                Count = 0,
                N = bufferSize,
                Full = new ConditionalVar() { WaitingQueue = new List<int>(), Name = "Full" },
                Empty = new ConditionalVar() { WaitingQueue = new List<int>(), Name = "Empty" }
            };
        }
    }
}
