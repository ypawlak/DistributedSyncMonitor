using DistributedMonitorMPI.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.ProducerConsumer
{

    [Serializable]
    public class ProdConsInternals
    {
        public ConditionalVar Full { get; set; }
        public ConditionalVar Empty { get; set; }
        public long Count { get; set; }
        public long N { get; set; }
    }
}
