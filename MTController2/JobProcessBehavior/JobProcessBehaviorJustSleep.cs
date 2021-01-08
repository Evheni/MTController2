using MTController2.JobInfo;
using MTController2.OptionClasses;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public class JobProcessBehaviorJustSleep : JobProcessBehavior
    {
        public JobProcessBehaviorJustSleep(Options options) : base(options)
        {
        }

        public override void ProcessSpecific(IJobInfo job) 
        {
            //await Task.Delay(10);
            Thread.Sleep(1000);
            Console.WriteLine($"job {(job as StringJobInfo).Job} after sleep");

        }
    }
}
