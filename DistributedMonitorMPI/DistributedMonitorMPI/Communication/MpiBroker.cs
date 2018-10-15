using DistributedMonitorMPI.Monitor;
using MPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DistributedMonitorMPI.Communication
{
    public class MpiBroker
    {
        private Intracommunicator _comm;

        public MpiBroker(Intracommunicator comm)
        {
            _comm = comm;
        }

        public IEnumerable<int> AllButMe
        {
            get { return Enumerable.Range(0, _comm.Size).Except(new List<int> { _comm.Rank }); }
        }

        public void Broadcast<T> (T message, int tag)
        {
            foreach (int proc in AllButMe)
            {
                Request req = _comm.ImmediateSend(message, proc, tag);
                req.Test();
            }
        }

        public bool ProbeMessage()
        {
            Status status = _comm.ImmediateProbe(Communicator.anySource, Communicator.anyTag);
            return status != null;
        }
    }
}
