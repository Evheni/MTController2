using MTController2.JobInfo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public class ProcessItemBehaviorJustSleep : IProcessItemBehavior
    {
        public void Process(IJobInfo job) 
        {
            //await Task.Delay(10);
            Thread.Sleep(10);
            Console.WriteLine($"job {(job as StringJobInfo).Job} after sleep");

        }
    }
}
