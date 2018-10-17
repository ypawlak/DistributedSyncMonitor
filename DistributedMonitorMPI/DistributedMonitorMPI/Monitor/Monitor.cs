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
        /// <summary>
        /// Critical section entry counter. Signifies which sequential entry to critical section are internals synchronized to.
        /// </summary>
        private long _syncEntryNumber { get; set; }

        private IList<MonitorMessage<T>> _deferredMessages = new List<MonitorMessage<T>>();
        protected Monitor(MpiHandler communicator)
        {
            Communicator = communicator;
            _syncEntryNumber = 0;
        }

        public void Communicate(long millisTimeout)
        {

            long startTime = DateTime.Now.Ticks;
            long elapsedTime = DateTime.Now.Ticks - startTime;

            while (elapsedTime < millisTimeout)
            {
                if (Communicator.ProbeMessage(Tags.PRIORITY_REQ_TAG))
                {
                    MonitorMessage<T> received = Communicator
                        .ReceiveMessage<MonitorMessage<T>>(Tags.PRIORITY_REQ_TAG);
                    ReplyAck(received);
                }

                if (Communicator.ProbeMessage(Tags.REQ_TAG))
                {
                    MonitorMessage<T> receivedReq = Communicator.ReceiveMessage<MonitorMessage<T>>(Tags.REQ_TAG);
                    ReplyAck(receivedReq);
                }

                elapsedTime = DateTime.Now.Ticks - startTime;
                //Console.WriteLine($"#{Communicator.MyRank} communicates, elapsed: {elapsedTime}");
            }
        }

        protected MpiHandler Communicator { get; set; }
        protected T Internals { get; set; }

        protected void Enter()
        {
            var req = BuildCurrentMonitorMessage();
            long sentClock = Communicator.Broadcast(req, Tags.REQ_TAG);

            Requesting(sentClock);
            
            _syncEntryNumber++;
            Logger.LogCSEntry(Communicator.MyRank, _syncEntryNumber, Communicator.Clock);
        }

        protected void Exit()
        {
            Logger.LogCSExit(Communicator.MyRank, _syncEntryNumber, Communicator.Clock);
            AckDeferredMessages();
        }

        protected void Wait(ConditionalVar condVar)
        {
            Logger.LogWait(Communicator.MyRank, _syncEntryNumber, Communicator.Clock);
            condVar.WaitingQueue.Add(Communicator.MyRank);
            AckDeferredMessages();
            Waiting();
            //add 1, it has to count as new entry
            _syncEntryNumber++;
        }
        protected void Signal(ConditionalVar condVar)
        {
            if(condVar.WaitingQueue.Any())
            {
                int destinationProc = condVar.WaitingQueue[0];
                condVar.WaitingQueue.RemoveAt(0);
                var msg = BuildCurrentMonitorMessage();
                Logger.LogPreSignal(Communicator.MyRank, destinationProc, _syncEntryNumber, Communicator.Clock);
                Communicator.Send(msg, destinationProc, Tags.WAKE_TAG);
                Communicator.Broadcast(msg, Tags.PRIORITY_REQ_TAG);
                PriorityRequesting();
                
                //add 1, it has to count as new entry
                _syncEntryNumber++;
                Logger.LogAfterSignalInCS(Communicator.MyRank, _syncEntryNumber, Communicator.Clock);
            }
        }

        private void Requesting(long sentRequestClock)
        {
            int receivedAcks = 0;
            T updatedInternals = Internals;
            long updatedSyncEntryNum = _syncEntryNumber;

            while(receivedAcks < Communicator.ProcessesCount - 1)
            {
                while (Communicator.ProbeMessage(Tags.ACK_TAG))
                {
                    MonitorMessage<T> received = Communicator
                        .ReceiveMessage<MonitorMessage<T>>(Tags.ACK_TAG);
                    
                    if (received.LastCsEntrySyncNumber > updatedSyncEntryNum)
                    {
                        //synchronizing current internal state to fresher critical section output
                        updatedInternals = received.InternalState;
                        updatedSyncEntryNum = received.LastCsEntrySyncNumber;
                    }
                    receivedAcks++;
                }

                if (Communicator.ProbeMessage(Tags.PRIORITY_REQ_TAG))
                {
                    MonitorMessage<T> received = Communicator
                        .ReceiveMessage<MonitorMessage<T>>(Tags.PRIORITY_REQ_TAG);
                    ReplyAck(received);
                }

                if (Communicator.ProbeMessage(Tags.REQ_TAG))
                {
                    MonitorMessage<T> received = Communicator
                        .ReceiveMessage<MonitorMessage<T>>(Tags.REQ_TAG);

                    if (IsPriorREQ(received, sentRequestClock))
                    {
                        ReplyAck(received);
                    }
                    else
                    {
                        _deferredMessages.Add(received);
                    }
                }
                Logger.LogRequestingWithReceivedAcks(Communicator.MyRank, receivedAcks, Communicator.Clock);
            }

            _syncEntryNumber = updatedSyncEntryNum;
            Internals = updatedInternals;
        }

        private bool IsPriorREQ(MonitorMessage<T> requestMsg, long comparedClockVal)
        {
            return requestMsg.Clock < comparedClockVal || requestMsg.Clock == comparedClockVal
                        && requestMsg.SenderRank < Communicator.MyRank;
        }

        private void Waiting()
        {
            while (!Communicator.ProbeMessage(Tags.WAKE_TAG))
            {
                if (Communicator.ProbeMessage(Tags.PRIORITY_REQ_TAG) && !Communicator.ProbeMessage(Tags.WAKE_TAG))
                {
                    MonitorMessage<T> received = Communicator
                        .ReceiveMessage<MonitorMessage<T>>(Tags.PRIORITY_REQ_TAG);
                    ReplyAck(received);
                }

                if (Communicator.ProbeMessage(Tags.REQ_TAG) && !Communicator.ProbeMessage(Tags.WAKE_TAG))
                {
                    MonitorMessage<T> receivedReq = Communicator.ReceiveMessage<MonitorMessage<T>>(Tags.REQ_TAG);
                    ReplyAck(receivedReq);
                }
            }

            MonitorMessage<T> awakeningMsg = Communicator.ReceiveMessage<MonitorMessage<T>>(Tags.WAKE_TAG);
            
            _syncEntryNumber = awakeningMsg.LastCsEntrySyncNumber;
            Internals = awakeningMsg.InternalState;
        }

        private void PriorityRequesting()
        {
            int receivedAcks = 0;
            T updatedInternals = Internals;
            long updatedSyncEntryNum = _syncEntryNumber;

            while(receivedAcks < Communicator.ProcessesCount - 1)
            {
                while (Communicator.ProbeMessage(Tags.ACK_TAG))
                {
                    MonitorMessage<T> received = Communicator
                        .ReceiveMessage<MonitorMessage<T>>(Tags.ACK_TAG);

                    if (received.LastCsEntrySyncNumber > updatedSyncEntryNum)
                    {
                        //synchronizing current internal state to fresher critical section output
                        updatedInternals = received.InternalState;
                        updatedSyncEntryNum = received.LastCsEntrySyncNumber;
                    }
                    receivedAcks++;
                }

                if (Communicator.ProbeMessage(Tags.REQ_TAG))
                {
                    MonitorMessage<T> received = Communicator
                        .ReceiveMessage<MonitorMessage<T>>(Tags.REQ_TAG);
                    _deferredMessages.Add(received);
                }

                if (Communicator.ProbeMessage(Tags.PRIORITY_REQ_TAG))
                {
                    MonitorMessage<T> received = Communicator
                        .ReceiveMessage<MonitorMessage<T>>(Tags.PRIORITY_REQ_TAG);
                    if (received.LastCsEntrySyncNumber > _syncEntryNumber)
                        ReplyAck(received);
                    else
                        _deferredMessages.Add(received);
                }
            }

            _syncEntryNumber = updatedSyncEntryNum;
            Internals = updatedInternals;

        }
        private void ReplyAck(MonitorMessage<T> msg)
        {
            var reply = BuildCurrentMonitorMessage();
            Communicator.Send(reply, msg.SenderRank, Tags.ACK_TAG);
        }

        private void AckDeferredMessages()
        {
            foreach (var message in _deferredMessages)
                ReplyAck(message);
            _deferredMessages.Clear();
        }

        private MonitorMessage<T> BuildCurrentMonitorMessage()
        {
            return new MonitorMessage<T>()
            {
                InternalState = Internals,
                LastCsEntrySyncNumber = _syncEntryNumber,
                SenderRank = Communicator.MyRank
            };
        }
    }
}
