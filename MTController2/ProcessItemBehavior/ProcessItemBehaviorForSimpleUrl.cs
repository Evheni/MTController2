using MTController2.JobInfo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public class ProcessItemBehaviorForSimpleUrl : IProcessItemBehavior
    {
        public void Process(IJobInfo job)
        {
            Console.WriteLine((job as StringJobInfo).Job);

        }
    }
}
