using MTController2.ExtraClasses;
using MTController2.JobInfo;
using MTController2.MultiThreadingController;
using MTController2.OptionClasses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    /// <summary>
    /// Class describing task processing behavior
    /// e.g. managing queue with tasks (enqueue tasks, dequeue tasks),
    /// organizing multithreaded task processing
    /// </summary>
    public abstract class Controller
    {
        /// <summary>Represents the option object which is used to initialize the controller
        /// e.g. database connection requisites etc.</summary>
        protected Options _options;
        
        /// <summary>
        /// threads processing jobs in simultaneously
        /// </summary>
        protected int _threadNumber;

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

        /// <summary>
        /// storing current controller state: INITIALIZATION, WORKING, STOPPING, PAUSED, PAUSING, DEINITIALIZATION
        /// </summary>
        protected ControllerStateManager _controllerStateManager;
      
        public string ControllerState
        {
            get { return _controllerStateManager.GetState().ToString(); }
        }

        /// <summary>
        /// event firing when all jobs are completed AND job queue is empty at the same time
        /// </summary>
        public event EventHandler<AllFinishedEventArgs> AllFinished;
        protected virtual void OnAllFinished(AllFinishedEventArgs e)
        {
            AllFinished?.Invoke(this, e);
        }

        private ProcessInfo mProcessInfo;

        public ProcessInfo ProcessInfo
        {
            get { return mProcessInfo; }
            set { mProcessInfo = value; }
        }

        /// <summary>
        /// queue storage
        /// </summary>
        protected ConcurrentQueue<IJobInfo> _queue { get; set; }

        /// <summary>
        /// behavior for processing every single job from queue
        /// Strategy pattern is used, so we can 
        /// </summary>
        protected JobProcessBehavior _processItemBehavior;

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

        /// <summary>
        /// Initializes a new instance of Controller
        /// </summary>
        /// <param name="threadNumber">number of threads to process jobs simultaneously</param>
        /// <param name="options">options for initialization, e.g. database requisites etc.</param>
        public Controller(JobProcessBehavior processItemBehavior, int threadNumber, Options options)
        {
            _threadNumber = threadNumber;
            _options = options;
            _stopCancellationTokenSource = new CancellationTokenSource();
            _pauseSignalOn = false;
            _controllerStateManager = new ControllerStateManager();

            _processItemBehavior = processItemBehavior;
            _queue = new ConcurrentQueue<IJobInfo>();

            _processItemBehavior.AddToQueue += _processItemBehavior_AddToQueue;
            mProcessInfo = new ProcessInfo();

        }

        /// <summary>
        /// Standard wrapper over initialization logic
        /// </summary>
        public virtual void Init()
        {
            _controllerStateManager.SetState(ProcessState.INITIALIZATION);
            InitSpecific();
        }

        /// <summary>
        /// Initialization logic, specific for particular class implementation
        /// </summary>
        protected abstract void InitSpecific();

        /// <summary>
        /// Standard wrapper over deinitialization logic
        /// </summary>
        public virtual void Deinit()
        {
            _controllerStateManager.SetState(ProcessState.DEINITIALIZATION);
            DeinitSpecific();
        }

        /// <summary>
        /// Deinitialization logic, specific for particular class implementation
        /// </summary>
        protected abstract void DeinitSpecific();

        /// <summary>
        /// Standard wrapper over deinitialization logic
        /// </summary>
        public virtual void Launch()
        {
            LaunchSpecific();
            _controllerStateManager.SetState(ProcessState.WORKING);
        }

        /// <summary>
        /// Launch job logic, specific for particular class implementation
        /// </summary>
        protected abstract void LaunchSpecific();

        /// <summary>
        /// Standard stop processing logic
        /// </summary>
        public virtual void Stop()
        {
            _controllerStateManager.SetState(ProcessState.STOPPING);
            _stopCancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Standard pause processing logic
        /// </summary>
        public virtual void Pause()
        {
            _controllerStateManager.SetState(ProcessState.PAUSING);
            _pauseSignalOn = true;
        }

        /// <summary>
        /// Standard resume processing logic
        /// </summary>
        public virtual void Resume()
        {
            _controllerStateManager.SetState(ProcessState.WORKING);
            _pauseSignalOn = false;
        }

        /// <summary>
        /// Waiting all job are processeв logic AND queue is empty
        /// specific for particular class implementation
        /// </summary>
        public abstract void WaitAllFinished();

        public virtual async void WaitAllFinishedAsync()
        {
            await Task.Run(WaitAllFinished);
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
    }

    public class AllFinishedEventArgs:EventArgs
    {
        public bool IsSuccess { get; private set; }
        public Exception Error { get; private set; }
        public AllFinishedEventArgs(bool isSuccess, Exception exp = null)
        {
            IsSuccess = isSuccess;
            Error = exp;
        }
    }
}
