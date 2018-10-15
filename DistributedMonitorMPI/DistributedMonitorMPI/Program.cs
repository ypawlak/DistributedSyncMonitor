using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPI;
using DistributedMonitorMPI.Monitor;

namespace DistributedMonitorMPI
{
    class Program
    {

        static void Main(string[] args)
        {
            SerializationTest(args);
        }

        private static void SerializationTest(string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                Intracommunicator comm = Communicator.world;
                if (comm.Rank == 0)
                {
                    ConditionVar testCon = new ConditionVar
                    {
                        WaitingQueue = new List<int> { 1, 2, 3, 4, 5 }
                    };
                    Request req = comm.ImmediateSend(testCon, 1, 0);
                    req.Test();
                }
                else
                {
                    ConditionVar msg = comm.Receive<ConditionVar>(Communicator.anySource, Communicator.anyTag);
                    Console.WriteLine(string.Format("#{0} received message [{1}]", comm.Rank, msg.WaitingQueue[0]));
                }
            }
        }
        private static void MpiTest(string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                Console.WriteLine("Rank: " +
                    Communicator.world.Rank + "(running on " + MPI.Environment.ProcessorName + ")");
                Intracommunicator comm = Communicator.world;
                if (comm.Rank == 0)
                {
                    Request req = comm.ImmediateSend("Rosie", 1, 0);
                    Console.WriteLine(req.Test() != null ? "COMPL" : "NOT COMPL");
                    string msg = comm.Receive<string>(Communicator.anySource, Communicator.anyTag);
                    Console.WriteLine(string.Format("#{0} received message [{1}]", comm.Rank, msg));
                    req = comm.ImmediateSend("Rosie", 1, 0);
                    Console.WriteLine(req.Test() != null ? "COMPL" : "NOT COMPL");
                    msg = comm.Receive<string>(Communicator.anySource, Communicator.anyTag);
                    Console.WriteLine(string.Format("#{0} received message [{1}]", comm.Rank, msg));
                }
                else
                {
                    string msg = comm.Receive<string>(comm.Rank - 1, 0);
                    Console.WriteLine(string.Format("#{0} received message [{1}]", comm.Rank, msg));
                    Request req = comm.ImmediateSend(msg + "," + comm.Rank, (comm.Rank + 1) % comm.Size, 0);
                    Console.WriteLine(req.Test() != null ? "COMPL" : "NOT COMPL");
                    msg = comm.Receive<string>(comm.Rank - 1, 0);
                    Console.WriteLine(string.Format("#{0} received message [{1}]", comm.Rank, msg));
                    req = comm.ImmediateSend(msg + "," + comm.Rank, (comm.Rank + 1) % comm.Size, 0);
                    Console.WriteLine(req.Test() != null ? "COMPL" : "NOT COMPL");
                }
            }
        }
    }
}
