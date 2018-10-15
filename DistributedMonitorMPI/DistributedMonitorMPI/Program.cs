using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPI;
using DistributedMonitorMPI.Monitor;
using DistributedMonitorMPI.ProducerConsumer;

namespace DistributedMonitorMPI
{
    class Program
    {

        static void Main(string[] args)
        {
            SerializationTest2(args);
        }

        private static void SerializationTest(string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                Intracommunicator comm = Communicator.world;
                if (comm.Rank == 0)
                {
                    ConditionalVar testCon = new ConditionalVar
                    {
                        WaitingQueue = new List<int> { 1, 2, 3, 4, 5 }
                    };
                    Request req = comm.ImmediateSend(testCon, 1, 0);
                    req.Test();
                }
                else
                {
                    ConditionalVar msg = comm.Receive<ConditionalVar>(Communicator.anySource, Communicator.anyTag);
                    Console.WriteLine(string.Format("#{0} received message [{1}]", comm.Rank, msg.WaitingQueue[0]));
                }
            }
        }

        private static void SerializationTest2(string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                Intracommunicator comm = Communicator.world;
                if (comm.Rank == 0)
                {
                    var test = new MonitorMessage<ProdConsInternals>()
                    {
                        InternalState = new ProdConsInternals()
                        {
                            Count = 0,
                            N = 100,
                            Full = new ConditionalVar() { WaitingQueue = new List<int>() { 1 } },
                            Empty = new ConditionalVar() { WaitingQueue = new List<int>() { 2 } }
                        },
                        EntryClock = 0,
                        State = State.INSIDE
                    };

                    
                    Request req = comm.ImmediateSend(test, 1, 0);
                    req.Test();
                }
                else
                {
                    MonitorMessage<ProdConsInternals> msg = comm.Receive<MonitorMessage<ProdConsInternals>>(Communicator.anySource, Communicator.anyTag);
                    Console.WriteLine(string.Format("#{0} received message [{1}]", comm.Rank, msg.State));
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
