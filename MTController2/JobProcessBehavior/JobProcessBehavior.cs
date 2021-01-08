using MTController2.JobInfo;
using MTController2.OptionClasses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public abstract class JobProcessBehavior
    {
        /// <summary>
        /// Item processing options.
        /// E.g. for parser it could be data extraction selectors (signatures) like xpath or regex
        /// </summary>
        protected Options _options;

        /// <summary>
        /// Delegate to be invoked when while processing item we need to add 
        /// new item to queue. E.g. when while extracting content from webpage,
        /// parser found links to other pages (and they should be enqueued)
        /// </summary>
        public event EventHandler<AddToQueueEventArgs> AddToQueue;

        /// <summary>
        /// Invoking add to queue delegate
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAddToQueue(AddToQueueEventArgs e)
        {
            AddToQueue?.Invoke(this, e);
        }
        public JobProcessBehavior(Options options)
        {
            options = _options;
        }

        /// <summary>
        /// The item processing function
        /// </summary>
        /// <param name="job"></param>
        protected virtual void Process(IJobInfo job)
        {
            ProcessSpecific(job);
        }

        public abstract void ProcessSpecific(IJobInfo job);
    }

    public class AddToQueueEventArgs : EventArgs
    {
        public IJobInfo Job { get; private set; }

        public AddToQueueEventArgs(IJobInfo job)
        {
            Job = job;
        }
    }
}