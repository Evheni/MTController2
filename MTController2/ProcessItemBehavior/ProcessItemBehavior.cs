using MTController2.JobInfo;
using MTController2.OptionClasses;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public abstract class ProcessItemBehavior 
    {
        protected Options _options;

        public ProcessItemBehavior(Options options)
        {
            options = _options;
        }
        public abstract void Process(IJobInfo job);
    }
}