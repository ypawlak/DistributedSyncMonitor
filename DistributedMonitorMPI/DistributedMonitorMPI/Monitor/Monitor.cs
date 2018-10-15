using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPI;
using DistributedMonitorMPI.Communication;

namespace DistributedMonitorMPI.Monitor
{
    /// <summary>
    /// Monitor synchoronization mechanism for distributed environment
    /// </summary>
    public abstract class Monitor<T>
    {
        private State _currentState;
        /// <summary>
        /// Critical section entry counter. Signifies which sequential entry to critical section are internals synchronized to.
        /// </summary>
        private long _syncEntryNumber { get; set; }

        private IList<MonitorMessage<T>> _deferredMessages = new List<MonitorMessage<T>>();
        protected Monitor(MpiBroker communicator)
        {
            Communicator = communicator;
            _currentState = State.OUTSIDE;
            _syncEntryNumber = 0;
        }

        protected MpiBroker Communicator { get; set; }
        protected T Internals { get; set; }
        protected void Enter()
        {
            var req = new MonitorMessage<T>()
            {
                InternalState = Internals,
                EntryClock = _syncEntryNumber,
                SenderRank = Communicator.MyRank
            };

            Communicator.Broadcast(req, Consts.REQ_TAG);
            _currentState = State.REQUESTING;

            Requesting();
            
            _syncEntryNumber++;
            Console.WriteLine(string.Format("#{0} made {1} entry to CS", Communicator.MyRank, _syncEntryNumber));
        }

        protected void Exit()
        {
            foreach(var message in _deferredMessages)
                ReplyAck(message);
            _deferredMessages.Clear();
            Console.WriteLine(string.Format("#{0} finished {1} entry to CS", Communicator.MyRank, _syncEntryNumber));
        }

        private void Requesting()
        {
            long receivedAcks = 0;
            T updatedInternals = Internals;
            long updatedSyncEntryNum = _syncEntryNumber;

            while(receivedAcks < Communicator.ProcessesCount - 1)
            {
                while (Communicator.ProbeMessage(Consts.ACK_TAG))
                {
                    MonitorMessage<T> received = Communicator
                        .ReceiveMessage<MonitorMessage<T>>(Consts.ACK_TAG);
                    
                    if (received.EntryClock > updatedSyncEntryNum)
                    {
                        //synchronizing current internal state to fresher critical section output
                        updatedInternals = received.InternalState;
                        updatedSyncEntryNum = received.EntryClock;
                    }
                    receivedAcks++;
                }

                if (Communicator.ProbeMessage(Consts.REQ_TAG))
                {
                    MonitorMessage<T> received = Communicator
                        .ReceiveMessage<MonitorMessage<T>>(Consts.REQ_TAG);

                    if (received.EntryClock > _syncEntryNumber)
                    {
                        ReplyAck(received);
                        Internals = received.InternalState;
                        _syncEntryNumber = received.EntryClock;
                    }
                    else if(received.EntryClock == _syncEntryNumber && received.SenderRank < Communicator.MyRank)
                    {
                        ReplyAck(received);
                    }
                    else
                    {
                        _deferredMessages.Add(received);
                    }
                }
            }

            _syncEntryNumber = updatedSyncEntryNum;
            Internals = updatedInternals;
        }

        private void Responding()
        {
            if (_currentState == State.OUTSIDE || _currentState == State.WAITING)
            {
                while (Communicator.ProbeMessage(Consts.REQ_TAG))
                {
                    MonitorMessage<T> received = Communicator.ReceiveMessage<MonitorMessage<T>>(Consts.REQ_TAG);
                    ReplyAck(received);
                }
            }
        }
        private void ReplyAck(MonitorMessage<T> msg)
        {
            var reply = new MonitorMessage<T>()
            {
                InternalState = Internals,
                EntryClock = _syncEntryNumber,
                SenderRank = Communicator.MyRank
            };
            Communicator.Send(reply, msg.SenderRank, Consts.ACK_TAG);
        }

        //public void Signal(ConditionVar condVar)
        //{

        //}

        //public void Wait(ConditionVar condVar)
        //{

        //}
    }
}
