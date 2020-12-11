using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JobRun
{
    public class JobWorker
    {
        private List<Thread>        _list;
        private CancellationToken   token;
        
        private ConcurrentQueue<IJob> Queue{ get; set; }
        
        public JobWorker(int threadPerProcessorCore)
        {
            Queue = new ConcurrentQueue<IJob>();
            int count = 10;
                //threadPerProcessorCore switch
                //        {
                //            <=0 or >10 => Environment.ProcessorCount,
                //            _ => Environment.ProcessorCount * threadPerProcessorCore
                //        };
            _list = new List<Thread>();

            for (int i = 0; i < count; i++)
            {
                var t = new Thread(MainWorker)
                        {
                            IsBackground = true,
                            Priority = ThreadPriority.Normal,
                        };
                t.Name = "JobWorker" + t.ManagedThreadId;
                _list.Add(t);
            }
            _list.ForEach(x=>x.Start());
        }

        public void AddJob(IJob job)
        {
           Queue.Enqueue(job);
        }
        
        private void MainWorker()
        {
            Console.WriteLine(Thread.CurrentThread.Name);
            while (!token.IsCancellationRequested)
            {
                if (Queue.TryDequeue(out var job))
                {
                    job.Execute(Thread.CurrentThread.Name);
                }
                Thread.Sleep(100);
            }
        }
    }
    public interface IJob
    {
        void SetSettings(object settings);
        void Execute(string s);
    }
}