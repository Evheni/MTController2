using MTController2.Exp2;
using MTController2.ExtraClasses;
using MTController2.JobInfo;
using MTController2.OptionClasses;
using MTController2.ProcessItemBehaviorNS;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MTController2.MultiThreadingController
{
    /// <summary>
    /// Controller implementation based on queue.
    /// Queue is used to enqueue and dequeue jobs
    /// </summary>
    public abstract class QueueBasedController : Controller
    {
        /// <summary>
        /// queue storage
        /// </summary>
        protected ConcurrentQueue<IJobInfo> _queue { get; set; }

        /// <summary>
        /// behavior for processing every single job from queue
        /// Strategy pattern is used, so we can 
        /// </summary>
        protected QueueBasedProcessItemBehavior _processItemBehavior;

        /// <summary>
        /// counter to track number of items, which are being processed by threads right in the current moment
        /// </summary>
        protected int _currentlyProcessingItemCount;
        /// <summary>
        /// counter to track number of items, which processing is already paused considering _pauseSignalOn state
        /// it helps to distinguish between PAUSING and PAUSED controller states
        /// </summary>
        protected int _pausedProcessingItemCount;

        /// <summary>
        /// locker to synchronize _pausedProcessingItemCount access
        /// </summary>
        protected object _pauseLockerObject = new object();

        public QueueBasedController(QueueBasedProcessItemBehavior processItemBehavior, int threadNumber, Options options)
            : base(threadNumber, options)
        {
            _processItemBehavior = processItemBehavior;
            _queue = new ConcurrentQueue<IJobInfo>();

            _processItemBehavior.AddToQueue += _processItemBehavior_AddToQueue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _processItemBehavior_AddToQueue(object sender, AddToQueueEventArgs e)
        {
            _queue.Enqueue(e.Job);
        }

        protected virtual void ProcessItem(IJobInfo job)
        {
            HandlePause();
            
            _processItemBehavior.Process(job);
        }
        
        void HandlePause()
        {
            if (!_pauseSignalOn || _stopCancellationTokenSource.IsCancellationRequested) return;
           
            lock (_pauseLockerObject)
            {
                _pausedProcessingItemCount++;

                if (_pausedProcessingItemCount == _currentlyProcessingItemCount) _controllerStateManager.SetState(ProcessState.PAUSED);
            }
            while (true)
            {
                if (_pauseSignalOn && !_stopCancellationTokenSource.IsCancellationRequested)
                {
                    Thread.Sleep(500);
                }
                else
                {
                    break;
                }
            }
            lock (_pauseLockerObject)
            {
                _pausedProcessingItemCount--;
            }
        }


        public virtual void FillQueue(List<IJobInfo> jobs)
        {
            foreach (var job in jobs)
            {
                _queue.Enqueue(job);
            }
        }

        public virtual void AddToQueue(IJobInfo job)
        {
            _queue.Enqueue(job);
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
