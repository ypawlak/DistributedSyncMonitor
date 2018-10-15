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
        private MpiBroker _communicator;

        private State _currentState;
        private long _syncEntryNumber { get; set; }
        public Monitor(MpiBroker communicator)
        {
            _communicator = communicator;
            _currentState = State.OUTSIDE;
            _syncEntryNumber = 0;
        }

        protected T Internals { get; set; }
        public void Enter()
        { 
            var req = new MonitorMessage<T>()
            {
                InternalState = Internals,
                EntryClock = _syncEntryNumber,
                State = _currentState
            };

            _communicator.Broadcast(req, Consts.REQ_TAG);
            //wait for all

        }

        public void Exit()
        {

        }

        public void ListenerJob()
        {
            while(_communicator.ProbeMessage())
            {

            }
        }

        private 

        //public MonitorDTO Signal(ConditionVar condVar)
        //{

        //}

        //public MonitorDTO Wait(ConditionVar condVar)
        //{

        //}
    }
}
