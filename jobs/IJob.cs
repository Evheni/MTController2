using System.Diagnostics;
using Core.Jobs.Services;

namespace Core.Jobs
{
    public interface IJob
    {
        void SetSettings(object settings);
        object Execute();
    }
}