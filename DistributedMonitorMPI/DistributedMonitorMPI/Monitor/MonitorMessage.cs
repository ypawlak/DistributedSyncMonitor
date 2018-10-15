using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.Monitor
{
    [Serializable]
    public class MonitorMessage
    {
        public MonitorDTO InternalState { get; set; }
        public long EntryClock { get; set; }
        public long State { get; set; }
    }
}
