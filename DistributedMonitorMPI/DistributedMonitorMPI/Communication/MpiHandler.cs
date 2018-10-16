using DistributedMonitorMPI.Monitor;
using MPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DistributedMonitorMPI.Communication
{
    public class MpiHandler
    {
        private Intracommunicator _comm;

        public MpiHandler(Intracommunicator comm)
        {
            _comm = comm;
        }

        public int MyRank => _comm.Rank;
        public int ProcessesCount => _comm.Size;
        public IEnumerable<int> AllButMe => Enumerable.Range(0, _comm.Size).Except(new List<int> { _comm.Rank });
        

        public void Send<T> (T message, int to, int tag)
        {
            Request sent =_comm.ImmediateSend(message, to, tag);
            sent.Test();
            Logger.LogSent(MyRank, to, tag);
        }

        public void Broadcast<T> (T message, int tag)
        {
            foreach (int proc in AllButMe)
                Send(message, proc, tag);
        }

        public bool ProbeMessage()
        {
            Status status = _comm.ImmediateProbe(Communicator.anySource, Communicator.anyTag);
            return status != null;
        }

        public bool ProbeMessage(int tag)
        {
            Status status = _comm.ImmediateProbe(Communicator.anySource, tag);
            return status != null;
        }

        public T ReceiveMessage<T> (int tag)
        {
            _comm.Receive<T>(Communicator.anySource, tag, out T rcvdMsg, out CompletedStatus rcvdStatus);
            Logger.LogReceived(MyRank, rcvdStatus.Source, rcvdStatus.Tag);
            return rcvdMsg;
        }
    }
}
