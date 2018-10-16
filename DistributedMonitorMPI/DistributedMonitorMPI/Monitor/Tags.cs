using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistributedMonitorMPI.Monitor
{
    public class Tags
    {
        public const int REQ_TAG = 10;
        public const int ACK_TAG = 11;
        public const int WAKE_TAG = 12;
        public const int PRIORITY_REQ_TAG = 13;

        public static readonly IDictionary<int, string> TagsDict = new Dictionary<int, string>
            {
                {REQ_TAG, "REQ"},
                {ACK_TAG, "ACK" },
                {WAKE_TAG, "WAKE" },
                {PRIORITY_REQ_TAG, "PRIORITY_REQ" }
            };
    }
}
