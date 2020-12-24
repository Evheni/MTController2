using MTController2.Exp2;
using MTController2.JobInfo;
using MTController2.OptionClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTController2.ProcessItemBehaviorNS
{
    /// <summary>
    /// Classes, encapsulation single job processing logic
    /// </summary>
    public abstract class QueueBasedProcessItemBehavior : ProcessItemBehavior
    {
        protected QueueBasedProcessItemBehavior(Options options) : base(options)
        {
        }

        public event EventHandler<AddToQueueEventArgs> AddToQueue;

        protected virtual void OnAllFinished(AddToQueueEventArgs e)
        {
            AddToQueue?.Invoke(this, e);
        }

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
