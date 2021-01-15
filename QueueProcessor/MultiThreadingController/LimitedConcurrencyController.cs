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
        #region Properties
        /// <summary>
        /// Task factory to implement multithreaded processing for jobs, extracted from queue
        /// </summary>
        private readonly TaskFactory _localTaskFactory;

        /// <summary>
        /// Tasks created with StartNew function of TaskFactory
        /// We need to store them to be able to wait for all finished
        /// </summary>
        private List<Task> _executedTasks;

        /// <summary>
        /// Needed for correct handlihg of waiting for all finished
        /// </summary>
        private int _taskCounter;

        /// <summary>
        /// counter to track number of items, which are being processed by threads right in the current moment
        /// </summary>
        protected int _currentlyProcessingItemCount;
        /// <summary>
        /// counter to track number of items, which processing is already paused considering _pauseSignalOn state
        /// it helps to distinguish between PAUSING and PAUSED controller states
        /// </summary>
        // protected int _pausedProcessingItemCount;

        /// <summary>
        /// locker to synchronize _pausedProcessingItemCount access
        /// </summary>
        protected object _currentProcessingItemLockerObject = new object();

        /// <summary>
        /// Cancellation token source used to stop the launched job
        /// If job was stopped - it cannot be resumed from the same point
        /// </summary>
        protected CancellationTokenSource _stopCancellationTokenSource;
        /// <summary>
        /// flag used to pause the launched job
        /// it can be later resumed from the same point
        /// </summary>
        protected bool _pauseSignalOn;

        #endregion

        public LimitedConcurrencyController(JobProcessBehavior processItemBehavior, int threadNumber, Options options)
            : base(processItemBehavior, threadNumber, options)
        {

            _localTaskFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(threadNumber));
            _executedTasks = new List<Task>();

            _taskCounter = 0;

            _stopCancellationTokenSource = new CancellationTokenSource();
            _pauseSignalOn = false;
        }

        void ProcessItemAsObject(object item)
        {
            
            ProcessItem((IJobInfo)item);
        }

        public override void Launch()
        {
            ProcessState state = _controllerStateManager.GetState();
            if (state != ProcessState.INITIALIZATION &&
                state != ProcessState.STOPPED)
                throw new NotAppropriateStateException();

            while (_queue.Count>0 && !_stopCancellationTokenSource.IsCancellationRequested)
            {
                //Interlocked.Increment(ref _taskCounter);
                lock (_currentProcessingItemLockerObject)
                {
                    _taskCounter++;
                }
                _queue.TryDequeue(out IJobInfo job);

                Task thisTask =
                    _localTaskFactory.StartNew(ProcessItemAsObject, job, _stopCancellationTokenSource.Token);

                lock (_currentProcessingItemLockerObject)
                {
                    ProcessInfo.ElementsInQueue++;
                }
                    //Interlocked.Increment(ref ProcessInfo.ElementsInQueue);

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
            _controllerStateManager.SetState(ProcessState.WORKING);
        }

        public override void Stop()
        {
            if (_controllerStateManager.GetState() != ProcessState.WORKING &&
                _controllerStateManager.GetState() != ProcessState.PAUSED &&
                _controllerStateManager.GetState() != ProcessState.PAUSING )
                throw new NotAppropriateStateException();
            _controllerStateManager.SetState(ProcessState.STOPPING);
            _stopCancellationTokenSource.Cancel();
        }
        public override void Pause()
        {
            if (_controllerStateManager.GetState() != ProcessState.WORKING)
                throw new NotAppropriateStateException();

            _controllerStateManager.SetState(ProcessState.PAUSING);
            _pauseSignalOn = true;
        }
        public override void Resume()
        {
            if (_controllerStateManager.GetState() != ProcessState.PAUSED &&
               _controllerStateManager.GetState() != ProcessState.PAUSING)
                throw new NotAppropriateStateException();
            _controllerStateManager.SetState(ProcessState.WORKING);
            _pauseSignalOn = false;
        }
        public override void Init()
        {
            _controllerStateManager.SetState(ProcessState.INITIALIZATION);
            Thread.Sleep(1000);
        }

        public override void Deinit()
        {
            _controllerStateManager.SetState(ProcessState.DEINITIALIZATION);
            Thread.Sleep(1000);
            // throw new NotImplementedException();
        }
        

        protected virtual void ProcessItem(IJobInfo job)
        {
            lock (_currentProcessingItemLockerObject)
            {
                _currentlyProcessingItemCount++;
            }
                        
            _processItemBehavior.ProcessSpecific(job);
            
            //Interlocked.Decrement(ref _taskCounter);
            //Interlocked.Decrement(ref ProcessInfo.ElementsInQueue);
            //Interlocked.Increment(ref ProcessInfo.Results);

            lock (_currentProcessingItemLockerObject)
            {
                Debug.WriteLine($"Thread {Thread.CurrentThread} changing counters");
                _taskCounter--;

                _currentlyProcessingItemCount--;
                ProcessInfo.ElementsInQueue--;
                ProcessInfo.Results++;
                Debug.WriteLine($"Thread {Thread.CurrentThread} _currentlyProcessingItemCount = {_currentlyProcessingItemCount}");
            }

            HandlePause();
        }

        void HandlePause()
        {
            
            if (!_pauseSignalOn || _stopCancellationTokenSource.IsCancellationRequested) return;

            lock (_currentProcessingItemLockerObject)
            {
                // _pausedProcessingItemCount++;

                //if (_pausedProcessingItemCount == _currentlyProcessingItemCount) {
                if (_currentlyProcessingItemCount == 0)
                {
                    Debug.WriteLine($"Thread {Thread.CurrentThread} switched controller to paused");
                    _controllerStateManager.SetState(ProcessState.PAUSED); 
                }
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
            //lock (_pauseLockerObject)
            //{
            //    //_pausedProcessingItemCount--;
            //}
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

            if (_stopCancellationTokenSource.IsCancellationRequested)
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
