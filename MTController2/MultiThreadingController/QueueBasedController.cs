using MTController2.Exp2;
using MTController2.JobInfo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MTController2.MultiThreadingController
{
    public abstract class QueueBasedController : Controller
    {
        
        protected ConcurrentQueue<IJobInfo> _queue { get; set; }
        protected IProcessItemBehavior _processItemBehavior;
        public QueueBasedController(IProcessItemBehavior processItemBehavior, int threadNumber)
            : base(threadNumber)
        {
            _processItemBehavior = processItemBehavior;
            _queue = new ConcurrentQueue<IJobInfo>(); 
        }

        protected virtual void ProcessItem(IJobInfo job)
        {
            _processItemBehavior.Process(job);
        }

        public virtual void FillQueue(List<IJobInfo> jobs)
        {
            foreach (var job in jobs)
            {
                _queue.Enqueue(job);
            }
        }

        /// <summary>
        /// Check if queue is empty
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsQueueEmpty()
        {
            return _queue.Count == 0;
        }
    }
}
