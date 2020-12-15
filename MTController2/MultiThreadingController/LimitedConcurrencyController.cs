using MTController2.JobInfo;
using MTController2.MultiThreadingController;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public class LimitedConcurrencyController : QueueBasedController
    {
        private readonly TaskFactory _localTaskFactory;
        private List<Task> _executedTasks;
        
        private int _taskCounter;

        
        public LimitedConcurrencyController(IProcessItemBehavior processItemBehavior, int threadNumber)
            : base(processItemBehavior, threadNumber)
        {
            _localTaskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(threadNumber));
            _executedTasks = new List<Task>();
            
            _taskCounter = 0;
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
            ProcessItem((IJobInfo)item);

            Interlocked.Decrement(ref _taskCounter);
        }
   

        public override void Launch()
        {
            while(_queue.Count>0)
            {
                Interlocked.Increment(ref _taskCounter);

                _queue.TryDequeue(out IJobInfo job);
                
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

    }
}
