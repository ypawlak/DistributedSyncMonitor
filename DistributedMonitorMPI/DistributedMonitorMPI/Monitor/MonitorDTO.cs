using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DistributedMonitorMPI.Monitor
{
    [Serializable]
    public class MonitorDTO
    {
        IDictionary<string, ConditionVar> ConditionVars { get; set; }
        IList<ISerializable> PrivateObjects { get; set; }
    }
}
