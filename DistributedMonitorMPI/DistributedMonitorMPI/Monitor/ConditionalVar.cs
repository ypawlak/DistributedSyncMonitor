using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.Monitor
{
    [Serializable]
    public class ConditionalVar
    {
        //For logging
        public string Name { get; set; }
        public IList<int> WaitingQueue { get; set; }
    }
}
