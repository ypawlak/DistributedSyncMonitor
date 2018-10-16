using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DistributedMonitorMPI.Monitor;
using DistributedMonitorMPI.Communication;

namespace DistributedMonitorMPI.ProducerConsumer
{
    public class ProdConsSyncBuffer<T> : Monitor.Monitor<ProdConsInternals<T>>
    {
        public ProdConsSyncBuffer(MpiHandler communicator, int bufferSize) : base(communicator)
        {
            Internals = new ProdConsInternals<T>()
            {
                N = bufferSize,
                Buffer = new List<T>(),
                Full = new ConditionalVar() { WaitingQueue = new List<int>(), Name = "Full" },
                Empty = new ConditionalVar() { WaitingQueue = new List<int>(), Name = "Empty" }
            };
        }

        public void Put(T item)
        {
            Enter();

            if (Internals.Buffer.Count == Internals.N)
                Wait(Internals.Full);
            Internals.Buffer.Add(item);
            Signal(Internals.Empty);

            Exit();
        }

        public T Get()
        {
            Enter();

            if (!Internals.Buffer.Any())
                Wait(Internals.Empty);
            T result = Internals.Buffer.Last();
            Internals.Buffer.RemoveAt(Internals.Buffer.Count - 1);
            Signal(Internals.Full);

            Exit();
            return result;
        }
    }
}
