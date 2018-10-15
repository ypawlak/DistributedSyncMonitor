using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPI;
using DistributedMonitorMPI.Communication;

namespace DistributedMonitorMPI.Monitor
{
    /// <summary>
    /// Synchronization monitor using MPI Intracommunicator (distributed environment)
    /// </summary>
    public abstract class Monitor
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

        public abstract MonitorDTO ToDTO();
        public abstract void UpdateWith(MonitorDTO dto);
        public void Enter()
        {
            MonitorDTO current = ToDTO();

        }

        public void Exit()
        {

        }

        //public MonitorDTO Signal(ConditionVar condVar)
        //{

        //}

        //public MonitorDTO Wait(ConditionVar condVar)
        //{

        //}
    }
}
