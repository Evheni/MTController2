using MTController2.JobInfo;
using MTController2.OptionClasses;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    /// <summary>
    /// Job processor implementation for job, which is represented just by string url
    /// </summary>
    public class JobProcessBehaviorForSimpleUrl : JobProcessBehavior
    {
        public JobProcessBehaviorForSimpleUrl(Options options) :base(options)
        {

        }
        public override void ProcessSpecific(IJobInfo job)
        {
            Console.WriteLine((job as StringJobInfo).Job);
            //load
            //extract links
            //extract data
            //export
        }
    }
}
