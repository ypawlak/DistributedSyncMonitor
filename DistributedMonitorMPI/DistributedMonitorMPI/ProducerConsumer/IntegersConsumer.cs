using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DistributedMonitorMPI.ProducerConsumer
{
    class IntegersConsumer
    {
        private ProdConsSyncBuffer<int> _buffer;
        private int _rank;

        public IntegersConsumer(int consumerRank, ProdConsSyncBuffer<int> buffer)
        {
            _buffer = buffer;
            _rank = consumerRank;
        }
        public void Consume(long valuesCount)
        {
            int consumedCount = 0;
            while (valuesCount > consumedCount)
            {
                //Communicating
                Console.WriteLine(string.Format($"Consumer #{_rank} begun communication"));
                _buffer.Communicate( new Random().Next(200, 100000));
                
                Console.WriteLine(string.Format($"Consumer #{_rank} begun obtaining value from buffer"));
                int consumedVal = _buffer.Get();
                Console.WriteLine(string.Format($"Consumer #{_rank} obtained value [{consumedVal}]"));
                
                //Consuming
                //Thread.Sleep(new Random().Next(200, 1000));
                consumedCount++;
                //Console.WriteLine(string.Format($"Consumer #{_rank} consumed value [{consumedVal}]"));
            }

            //Console.WriteLine(string.Format($"Consumer #{_rank} begun POST WORK communication"));
            _buffer.Communicate(1000000);
            Console.WriteLine(string.Format($"Consumer #{_rank} finished POST WORK communication"));
        }
    }
}
