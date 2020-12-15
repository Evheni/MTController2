using MTController2.Exp2;
using MTController2.JobInfo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.MultiThreadingController
{
    public class JobWorkerController : QueueBasedController
    {
        private List<Thread> _list;
        private object locker = new object();
        bool _sendOnQueueEmpty = false;


        public JobWorkerController(IProcessItemBehavior processItemBehavior, int threadNumber)
            : base(processItemBehavior, threadNumber)
        {
            
        }


        private void MainWorker()
        {
            while (!_moduleCancellationToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var jobinfo))
                {
                    lock(locker) _sendOnQueueEmpty = _queue.Count == 0;

                    try
                    {
                        ProcessItem(jobinfo); 
                    }
                    catch (Exception e)
                    {
                      
                    }
                    lock (locker)
                    {
                        if (_sendOnQueueEmpty)
                        {
                            OnAllFinished(new AllFinishedEventArgs(true));
                        }
                    }
                }

                /*else
                {
                        if (CurrentJobs.Count == 0)
                        {
                            if (_sendOnQueueEmpty)
                            {
                                
                                _sendOnQueueEmpty = false;
                            }
                        }
                }*/
                Thread.Sleep(10);
            }
        }
        public override void Launch()
        {

            _list = new List<Thread>();
            for (int i = 0; i < _threadNumber; i++)
            {
                var t = new Thread(MainWorker)
                {
                    IsBackground = false,
                    Priority = ThreadPriority.Normal,
                };
                t.Name = "JobWorker" + t.ManagedThreadId;
                _list.Add(t);
            }

            _list.ForEach(x => x.Start());

        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override void Resume()
        {
            throw new NotImplementedException();
        }

        public override void WaitAllFinished()
        {
            while(true)
            {
                lock(locker)
                {
                    if (_sendOnQueueEmpty) break;
                }

                Thread.Sleep(10);
            }
            //Можно ли здесь ожидать события?
            //throw new NotImplementedException();
        }

    }
}
