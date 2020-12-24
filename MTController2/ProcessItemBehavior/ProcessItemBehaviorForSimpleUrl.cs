using MTController2.JobInfo;
using MTController2.OptionClasses;
using MTController2.ProcessItemBehaviorNS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public class ProcessItemBehaviorForSimpleUrl : QueueBasedProcessItemBehavior
    {
        public ProcessItemBehaviorForSimpleUrl(Options options) :base(options)
        {

        }
        public override void Process(IJobInfo job)
        {
            Console.WriteLine((job as StringJobInfo).Job);

        }
    }
}
