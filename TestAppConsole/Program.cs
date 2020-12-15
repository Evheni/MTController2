using MTController2.Exp2;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Jobs;
using MTController2.JobInfo;
using MTController2.MultiThreadingController;

namespace TestAppConsole
{
    class Program
    {
        private async static void Run()
        {
            var ct = new CancellationToken();

            Console.WriteLine("Hello World!");
            Console.WriteLine($"Run():{Thread.CurrentThread.ManagedThreadId}");

            var j = new JobWorker(ct);

            //j.AddJobEvent   += info => Console.WriteLine($"Работа добавлена в очередь:{info.JobId} :{info.Name}");
            //j.StartJobEvent += info => Console.WriteLine($"Работа запущена:{info.JobId} :{info.Name} : {info.TimeStart}");
            //j.EndJobEvent   += info => Console.WriteLine($"EndJobEvent:{info.Result as string}");

            //j.ErrorEvent += info => Console.WriteLine($"ERROR :{info.JobId} :{info.Name}:{info.ErrorMessage}");
            j.QueueEmpty += () => Console.WriteLine($"Queue is empty");

            //await Task.Delay(1000, ct);

            Console.WriteLine("Start adding jobs");

            for (int i = 0; i < 10000; i++)
            {
                var job = new JobInfo
                {
                    Name = $"TestPaused-{i}",
                    JobClass = typeof(PausedJob),
                    Settings = "str"
                };

                j.AddJob(job);
            }
            /*
            var job1 = new JobInfo
                      {
                          Name     = $"TestError",
                          JobClass = typeof(ErrorJob),
                          Settings = "str"
                      };

            j.AddJob(job1);

            var job2 = new JobInfo
                       {
                           Name     = $"TestInterface",
                           JobClass = typeof(ErrorByInterfaceJob),
                           Settings = "str"
                       };

            j.AddJob(job2);
            */

        }

        static void Main(string[] args)
        {
            //Run();
            //Console.ReadLine();
            //return;
            
            int iterationNum = 100;
            
            DateTime startDt;

            #region QueueBasedController

            List<IJobInfo> inputQueue = new List<IJobInfo>();

            for (int i = 0; i < iterationNum; i++)
            {
                inputQueue.Add(new StringJobInfo(i.ToString()));
            }

            QueueBasedController mt = new LimitedConcurrencyController
                (
                    new ProcessItemBehaviorJustSleep(),
                    50                    
                );

            mt.FillQueue(inputQueue);

            startDt = DateTime.Now;

            mt.Launch();

            mt.WaitAllFinished();

            //GC.GetTotalMemory(true)
            Console.WriteLine($"{mt.GetType()} test: " + Math.Round((DateTime.Now - startDt).TotalMilliseconds) + " ms");

            #endregion

            #region No thread test

            //startDt = DateTime.Now;

            //for (int i = 0; i < iterationNum; i++)
            //{
            //    processItemBehavior.Process(i.ToString());
            //}

            //Console.WriteLine("No thread test: " + Math.Round((DateTime.Now - startDt).TotalMilliseconds) + " ms");

            #endregion



            Console.ReadKey();
        }

    }


    public class PausedJob : IJob
    {
        private string _settings;

        public void SetSettings(object settings)
        {
            _settings = settings as string;
        }

        public object Execute()
        {
            Console.WriteLine($"Execute(), thread id{Thread.CurrentThread.ManagedThreadId}");

            Thread.Sleep(10);
            return Thread.CurrentThread.ManagedThreadId.ToString();
        }
    }

    public class ErrorJob : IJob
    {
        private string _settings;

        public void SetSettings(object settings)
        {
            _settings = settings as string;
        }

        public object Execute()
        {
            var a = 0;
            var b = 1;
            var c = b / a;

            return true;
        }
    }


    public class ErrorByInterfaceJob
    {
        private string _settings;

        public void SetSettings(object settings)
        {
            _settings = settings as string;
        }

        public object Execute()
        {
            var a = 0;
            var b = 1;
            var c = b / a;

            return true;
        }
    }
}


