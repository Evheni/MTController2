using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    class LimitedConcurrencyController<T> : Controller<T>
    {
        private readonly TaskFactory _localTaskFactory;
        private List<Task> _executedTasks;
        private CancellationTokenSource _moduleCancellationToken;
        private int _taskCounter;
        private ConcurrentQueue<T> _jobs = new ConcurrentQueue<T>();
        public LimitedConcurrencyController(List<T>jobs,int threadNumber, IProcessItemBehavior<T> processItemBehavior)
            : base(threadNumber, processItemBehavior)
        {
            _localTaskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(threadNumber));
            _executedTasks = new List<Task>();
            _moduleCancellationToken = new CancellationTokenSource();
            _taskCounter = 0;
            foreach (var job in jobs)
            {
                _jobs.Enqueue(job);
            }
        }

        public override void WaitAllFinished()
        {
            while (true)
            {
                try
                {
                    Task.WaitAll(_executedTasks.ToArray());

                    if (_taskCounter > 0) continue;

                    Thread.Sleep(250);


                    if (!IsQueueEmpty())
                    {
                        Thread.Sleep(250);
                        continue;
                    }
                    OnAllFinished(new AllFinishedEventArgs(true));
                    break;
                }
                catch (Exception exp)
                {
                    Type expType = exp.GetType();
                    
                    OnAllFinished(new AllFinishedEventArgs(false, exp));

                    #region If task cancelled exception, just break cycle

                    if (exp.InnerException != null &&
                        exp.InnerException.GetType() == typeof(TaskCanceledException))
                    {
                        break;
                    }

                    #endregion

                    
                }
            }
        }

        public override async void WaitAllFinishedAsync()
        {
            await Task.Run(WaitAllFinished);
        }

        void ProcessItemAsObject(object item)
        {
            ProcessItem((T)item);

            Interlocked.Decrement(ref _taskCounter);
        }
        /// <summary>
        /// Check if queue is empty
        /// </summary>
        /// <returns></returns>
        private bool IsQueueEmpty()
        {
            return _jobs.Count == 0;
        }

        public override void Launch()
        {
            while(_jobs.Count>0)
            {
                Interlocked.Increment(ref _taskCounter);

                _jobs.TryDequeue(out T job);
                
                Task thisTask =
                    _localTaskFactory.StartNew(ProcessItemAsObject, job, _moduleCancellationToken.Token);

                _executedTasks.Add(thisTask);
            }
            
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override void Resume()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            _moduleCancellationToken.Cancel();
        }
    }
}
