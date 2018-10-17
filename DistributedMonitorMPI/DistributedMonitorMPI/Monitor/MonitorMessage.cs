using DistributedMonitorMPI.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DistributedMonitorMPI.Monitor
{
    [Serializable]
    public class MonitorMessage<T> : IMessage
    {
        public T InternalState { get; set; }
        public long LastCsEntrySyncNumber { get; set; }
        public int SenderRank { get; set; }
        public long Clock { get; set; }
    }
}
