using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.Monitor
{
    [Serializable]
    public class ConditionalVar
    {
        public IList<int> WaitingQueue { get; set; }
    }
}
