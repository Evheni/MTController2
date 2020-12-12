using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;

namespace Core.Jobs
{
    public class JobWorker
    {
        private List<Thread>      _list;
        private CancellationToken token;

        private ConcurrentQueue<JobInfo> Queue { get; set; }


        /*public event Action<JobInfo> AddJobEvent;
        public event Action<JobInfo> StartJobEvent;
        public event Action<JobInfo> EndJobEvent;*/

        public event Action<JobInfo> ErrorEvent;
        public event Action          QueueEmpty;
        private long                 NextJobId;

        public JobWorker(CancellationToken token, int count = 4, ThreadPriority priority = ThreadPriority.Normal)
        {
            NextJobId  = 1;
            this.token = token;
            Queue      = new ConcurrentQueue<JobInfo>();

            _list = new List<Thread>();
            for (int i = 0; i < count; i++)
            {
                var t = new Thread(MainWorker)
                        {
                            IsBackground = false,
                            Priority     = ThreadPriority.Normal,
                        };
                t.Name = "JobWorker" + t.ManagedThreadId;
                _list.Add(t);
            }

            _list.ForEach(x => x.Start());
        }

        public JobInfo AddJob(JobInfo job)
        {
            job.JobId     = NextJobId++;
            job.JobResult = JobResult.InQueue;

            Queue.Enqueue(job);

            // OnAddJobEvent(job);
            return job;
        }

        private void MainWorker()
        {
            while (!token.IsCancellationRequested)
            {
                if (Queue.TryDequeue(out var jobinfo))
                {
                    bool _sendOnQueueEmpty = Queue.Count == 0;
                    try
                    {

                        jobinfo.TimeStart = DateTime.Now;
                        jobinfo.TimeEnd   = jobinfo.TimeStart;
                        jobinfo.JobResult = JobResult.Started;
                        // OnStartJobEvent(jobinfo);

                        var job = (IJob) Activator.CreateInstance(jobinfo.JobClass);
                        jobinfo.Result = job.Execute();

                        jobinfo.TimeEnd   = DateTime.Now;
                        jobinfo.JobResult = JobResult.Completed;
                        //OnEndJobEvent(jobinfo);

                    }
                    catch (Exception e)
                    {
                        jobinfo.JobResult    = JobResult.Error;
                        jobinfo.ErrorMessage = e.Message;

                        OnErrorEvent(jobinfo);
                    }

                    if (_sendOnQueueEmpty)
                    {
                        OnQueueEmpty();
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

        protected virtual void OnQueueEmpty()
        {
            QueueEmpty?.Invoke();
        }

        /*protected virtual void OnEndJobEvent(JobInfo obj)
        {
            EndJobEvent?.Invoke(obj);
        }

        protected virtual void OnStartJobEvent(JobInfo obj)
        {
            StartJobEvent?.Invoke(obj);
        }

        protected virtual void OnAddJobEvent(JobInfo obj)
        {
            AddJobEvent?.Invoke(obj);
        }*/

        protected virtual void OnErrorEvent(JobInfo obj)
        {
            ErrorEvent?.Invoke(obj);
        }
    }
}