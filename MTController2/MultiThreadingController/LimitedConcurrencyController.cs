using MTController2.ExtraClasses;
using MTController2.JobInfo;
using MTController2.MultiThreadingController;
using MTController2.OptionClasses;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public class LimitedConcurrencyController : Controller
    {
        private readonly TaskFactory _localTaskFactory;
        private List<Task> _executedTasks;
        
        private int _taskCounter;

        
        public LimitedConcurrencyController(JobProcessBehavior processItemBehavior, int threadNumber, Options options)
            : base(processItemBehavior, threadNumber, options)
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
                    Debug.WriteLine("before Task.WaitAll");
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
                        Debug.WriteLine("TaskCanceledException");
                        break;
                    }

                    #endregion
                }

            }
            Deinit();

            if(_stopCancellationTokenSource.IsCancellationRequested)
            {
                _controllerStateManager.SetState(ProcessState.STOPPED);
            }
            else
            {
                _controllerStateManager.SetState(ProcessState.FINISHED);
            }
            
        }

        public override async void WaitAllFinishedAsync()
        {
            await Task.Run(WaitAllFinished);
        }

        void ProcessItemAsObject(object item)
        {
            Interlocked.Increment(ref _currentlyProcessingItemCount); 
            ProcessItem((IJobInfo)item);
            Interlocked.Decrement(ref _currentlyProcessingItemCount);

            Interlocked.Decrement(ref _taskCounter);
            Interlocked.Decrement(ref ProcessInfo.ElementsInQueue);
            Interlocked.Increment(ref ProcessInfo.Results);
        }

        protected override void LaunchSpecific()
        {
            while(_queue.Count>0 && !_stopCancellationTokenSource.IsCancellationRequested)
            {
                Interlocked.Increment(ref _taskCounter);

                _queue.TryDequeue(out IJobInfo job);

                // pause&stophandle

                // Debug.WriteLine($"before start new job: {job.ToString()}");
                Task thisTask =
                    _localTaskFactory.StartNew(ProcessItemAsObject, job, _stopCancellationTokenSource.Token);

                Interlocked.Increment(ref ProcessInfo.ElementsInQueue);

                _executedTasks.Add(thisTask);
            }
            if(_stopCancellationTokenSource.IsCancellationRequested)
            {
                Debug.WriteLine("LaunchSpecific interrupted by task cancellation");
            }
            else
            {
                Debug.WriteLine("LaunchSpecific finished");
            }
        }

        protected override void InitSpecific()
        {
            // throw new NotImplementedException();
        }

        protected override void DeinitSpecific()
        {
            Thread.Sleep(2000);
            // throw new NotImplementedException();
        }
        

        protected virtual void ProcessItem(IJobInfo job)
        {
            HandlePause();

            _processItemBehavior.ProcessSpecific(job);
        }

        void HandlePause()
        {
            if (!_pauseSignalOn || _stopCancellationTokenSource.IsCancellationRequested) return;

            lock (_pauseLockerObject)
            {
                _pausedProcessingItemCount++;

                if (_pausedProcessingItemCount == _currentlyProcessingItemCount) {
                    _controllerStateManager.SetState(ProcessState.PAUSED); }
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
