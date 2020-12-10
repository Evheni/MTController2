using System;
using System.Collections.Generic;
using System.Text;

namespace MTController2.Exp2
{
    public abstract class Controller <T>
    {
        protected int _threadNumber;
        IProcessItemBehavior<T> _processItemBehavior;
        public Controller(int threadNumber, IProcessItemBehavior<T> processItemBehavior)
        {
            _threadNumber = threadNumber;
            _processItemBehavior = processItemBehavior;
        }

        public abstract void Launch();

        public abstract void Stop();

        public abstract void Pause();

        public abstract void Resume();

        public abstract void WaitAllFinished();

        protected void ProcessItem(T job)
        {
            _processItemBehavior.Process(job);
        }
    }
}
