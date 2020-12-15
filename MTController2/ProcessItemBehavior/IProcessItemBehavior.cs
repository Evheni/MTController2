using MTController2.JobInfo;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public interface IProcessItemBehavior 
    {
        void Process(IJobInfo job);
    }
}