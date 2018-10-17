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
            Clock = 0;
        }

        public long Clock { get; private set; }

        public int MyRank => _comm.Rank;
        public int ProcessesCount => _comm.Size;
        public IEnumerable<int> AllButMe => Enumerable.Range(0, _comm.Size).Except(new List<int> { _comm.Rank });
        

        public long Send<T> (T message, int to, int tag) where T : IMessage
        {
            Clock++;
            message.Clock = Clock;
            Request sent =_comm.ImmediateSend(message, to, tag);
            sent.Test();
            Logger.LogSent(MyRank, to, tag, Clock);
            return Clock;
        }

        /// <summary>
        /// Broadcasts given message to all communicated processes
        /// </summary>
        /// <returns>Sent clock</returns>
        public long Broadcast<T> (T message, int tag) where T : IMessage
        {
            Clock++;
            message.Clock = Clock;
            foreach (int proc in AllButMe)
            {
                Request sent = _comm.ImmediateSend(message, proc, tag);
                sent.Test();
                Logger.LogSent(MyRank, proc, tag, Clock);
            }
            return Clock;
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

        public T ReceiveMessage<T> (int tag) where T : IMessage
        {
            _comm.Receive<T>(Communicator.anySource, tag, out T rcvdMsg, out CompletedStatus rcvdStatus);
            Clock = Math.Max(Clock, rcvdMsg.Clock) + 1;
            Logger.LogReceived(MyRank, rcvdStatus.Source, rcvdStatus.Tag, Clock);
            return rcvdMsg;
        }
    }
}
