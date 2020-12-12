using MTController2.Exp2;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Jobs;

namespace MTController2
{
    class Program
    {
        private async static void Run()
        {
            var ct = new CancellationToken();

            Console.WriteLine("Hello World!");

            var j = new JobWorker(ct);

            //j.AddJobEvent   += info => Console.WriteLine($"Работа добавлена в очередь:{info.JobId} :{info.Name}");
            //j.StartJobEvent += info => Console.WriteLine($"Работа запущена:{info.JobId} :{info.Name} : {info.TimeStart}");
            //j.EndJobEvent   += info => Console.WriteLine($"Работа окончена:{info.JobId} :{info.Name} : {info.TimeEnd} : Elapsed {info.TimeWork}");

            j.ErrorEvent += info => Console.WriteLine($"ERROR :{info.JobId} :{info.Name}:{info.ErrorMessage}");
            j.QueueEmpty += () => Console.WriteLine($"Очередь работ пуста");

           // await Task.Delay(2000, ct);
            
            Console.WriteLine("Начало добавления работ");
            for (int i = 0; i < 1000; i++)
            {
                var job = new JobInfo
                          {
                              Name     = $"TestPaused-{i}",
                              JobClass = typeof(PausedJob),
                              Settings = "str"
                          };

                j.AddJob(job);
            }

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


        }

        static void Main(string[] args)
        {
           Run();
           Console.ReadLine();
            return;
            
            
            
            
            int iterationNum = 10;
            IProcessItemBehavior<string> processItemBehavior = new ProcessItemBehaviorJustSleep();
                //new ProcessItemBehaviorForSimpleUrl();
            DateTime startDt;

            #region LimitedConcurrencyController

            List<string> inputQueue = new List<string>();

            for (int i = 0; i < iterationNum; i++)
            {
                inputQueue.Add(i.ToString());
            }

            Controller<string> mt = new LimitedConcurrencyController<string>
                (
                    inputQueue,
                    3,
                    new ProcessItemBehaviorForSimpleUrl()
                );


            startDt = DateTime.Now;

            mt.Launch();

            mt.WaitAllFinished();

            Console.WriteLine("LimitedConcurrencyController test: " + Math.Round((DateTime.Now - startDt).TotalMilliseconds) + " ms");

            #endregion


            #region No thread test

            startDt = DateTime.Now;

            for (int i = 0; i < iterationNum; i++)
            {
                processItemBehavior.Process(i.ToString());
            }

            Console.WriteLine("No thread test: " + Math.Round((DateTime.Now - startDt).TotalMilliseconds) + " ms");

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
            Thread.Sleep(10);
            return true;
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
