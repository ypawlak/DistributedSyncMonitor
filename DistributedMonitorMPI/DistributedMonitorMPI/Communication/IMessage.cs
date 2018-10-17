using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.Communication
{
    public interface IMessage
    {
        long Clock { get; set; }
    }
}
