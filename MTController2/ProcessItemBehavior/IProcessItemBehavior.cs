using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public interface IProcessItemBehavior <T>
    {
        void Process(T job);
    }
}