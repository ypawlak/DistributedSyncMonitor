using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DistributedMonitorMPI.Monitor
{
    [Serializable]
    public class MonitorMessage<T>
    {
        public T InternalState { get; set; }
        public long EntryClock { get; set; }
        public int SenderRank { get; set; }
    }
}
