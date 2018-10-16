using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DistributedMonitorMPI.Communication;

namespace DistributedMonitorMPI
{
    class TestMonitor : Monitor.Monitor<long>
    {
        public TestMonitor(MpiHandler communicator) : base(communicator)
        {
            Internals = 0;
        }

        public void SetRank()
        {
            Enter();
            Internals = Communicator.MyRank;
            Exit();
        }
    }
}
