using MTController2.JobInfo;
using MTController2.OptionClasses;
using MTController2.ProcessItemBehaviorNS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public class ProcessItemBehaviorJustSleep : QueueBasedProcessItemBehavior
    {
        public ProcessItemBehaviorJustSleep(Options options) : base(options)
        {
        }

        public override void Process(IJobInfo job) 
        {
            //await Task.Delay(10);
            Thread.Sleep(10);
            Console.WriteLine($"job {(job as StringJobInfo).Job} after sleep");

        }
    }
}
