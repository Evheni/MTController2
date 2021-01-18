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
    /// Multithreaded managing of task queue processing
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
        /// Initializes a new instance of Controller
        /// </summary>
        /// <param name="threadNumber">number of threads to process jobs simultaneously</param>
        /// <param name="options">options for initialization, e.g. database requisites etc.</param>
        public Controller(JobProcessBehavior processItemBehavior, int threadNumber, Options options)
        {
            _threadNumber = threadNumber;
            _options = options;
            
            _controllerStateManager = new ControllerStateManager();

            _processItemBehavior = processItemBehavior;
            _queue = new ConcurrentQueue<IJobInfo>();

            _processItemBehavior.AddToQueue += _processItemBehavior_AddToQueue;
            mProcessInfo = new ProcessInfo();

        }

        /// <summary>
        /// Controller nitialization logic
        /// </summary>
        public abstract void Init();


        /// <summary>
        /// Deinitialization logic
        /// </summary>
        public abstract void Deinit();

        /// <summary>
        /// Launch queue processing 
        /// </summary>
        public abstract void Launch();

        /// <summary>
        /// Terminate processing (after termination process cannot be resumed, only restarted)
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Pause processing  (after pausing process can be resumed)
        /// </summary>
        public abstract void Pause();

        /// <summary>
        /// Standard resume processing logic
        /// </summary>
        public abstract void Resume();

        /// <summary>
        /// Waiting all job are processeв logic AND queue is empty
        /// specific for particular class implementation
        /// </summary>
        public abstract void WaitAllFinished();

        public virtual async void WaitAllFinishedAsync()
        {
            await Task.Run((Action) WaitAllFinished);
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

}
