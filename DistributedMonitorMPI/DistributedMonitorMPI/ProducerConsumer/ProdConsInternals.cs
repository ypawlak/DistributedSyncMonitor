using DistributedMonitorMPI.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.ProducerConsumer
{

    [Serializable]
    public class ProdConsInternals<T>
    {
        public ConditionalVar Full { get; set; }
        public ConditionalVar Empty { get; set; }
        public int N { get; set; }
        public IList<T> Buffer { get; set; }
    }
}
