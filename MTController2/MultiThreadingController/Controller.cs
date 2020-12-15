using MTController2.JobInfo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    public abstract class Controller
    {
        //protected ConcurrentQueue<IJobInfo> _queue { get; set; }
        protected int _threadNumber;
        protected CancellationTokenSource _moduleCancellationToken;

        public Controller(int threadNumber)
        {
            _threadNumber = threadNumber;
            _moduleCancellationToken = new CancellationTokenSource();
        }

        public abstract void Launch();

        public virtual void Stop()
        {
            _moduleCancellationToken.Cancel();
        }

        public abstract void Pause();

        public abstract void Resume();

        public abstract void WaitAllFinished();

        public virtual async void WaitAllFinishedAsync()
        {
            await Task.Run(WaitAllFinished);
        }

        public event EventHandler<AllFinishedEventArgs> AllFinished;

        protected virtual void OnAllFinished(AllFinishedEventArgs e)
        {
            AllFinished?.Invoke(this,e);
        }
    }

    public class AllFinishedEventArgs:EventArgs
    {
        public bool IsSuccess { get; private set; }
        public Exception Error { get; private set; }
        public AllFinishedEventArgs(bool isSuccess, Exception exp = null)
        {
            IsSuccess = isSuccess;
            Error = exp;
        }
    }
}
