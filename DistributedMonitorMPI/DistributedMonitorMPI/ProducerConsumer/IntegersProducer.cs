using DistributedMonitorMPI.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DistributedMonitorMPI.ProducerConsumer
{
    public class IntegersProducer
    {
        private ProdConsSyncBuffer<int> _buffer;
        private int _rank;

        public IntegersProducer(int producerRank, ProdConsSyncBuffer<int> buffer)
        {
            _buffer = buffer;
            _rank = producerRank;
        }
        public void Produce(long valuesCount)
        {
            Random rand = new Random(_rank);
            int producedCount = 0;
            while (valuesCount > producedCount)
            {
                //Communicating
                //Console.WriteLine(string.Format($"Producer #{_rank} begun communication"));
                _buffer.Communicate(rand.Next(200, 100000));
                //Producing
                //Console.WriteLine(string.Format($"Producer #{_rank} begun producing"));
                Thread.Sleep(rand.Next(200, 1000));
                int newVal = rand.Next();
                //Console.WriteLine(string.Format($"Producer #{_rank} produced new value [{newVal}]"));
                _buffer.Put(newVal);
                producedCount++;
                //Console.WriteLine(string.Format($"Producer #{_rank} put new value [{newVal}] in buffer"));
            }

            Console.WriteLine(string.Format($"Producer #{_rank} begun POST WORK communication"));
            _buffer.Communicate(50000000);
            Console.WriteLine(string.Format($"Producer #{_rank} finished POST WORK communication"));
        }
    }
}
