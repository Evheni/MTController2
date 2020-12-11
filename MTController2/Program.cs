using JobRun;
using MTController2.Exp2;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MTController2
{
    class Program
    {
        static async void Run()
        {
            Console.WriteLine("Hello World!");

            var j = new JobWorker(1);
            //await Task.Delay(1000);
            for (int i = 0; i < 100; i++)
            {
                var job = new TestJob();
                job.SetSettings($"job-{i}");
                j.AddJob(job);
            }
            Console.WriteLine("hi");
            Console.ReadLine();


        }
        static void Main(string[] args)
        {
            //Run();
            //return;
            int iterationNum = 10;
           
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
                    5,
                    new ProcessItemBehaviorJustSleep()
                );


            startDt = DateTime.Now;

            mt.Launch();

            mt.WaitAllFinished();

            Console.WriteLine("LimitedConcurrencyController test: " + Math.Round((DateTime.Now - startDt).TotalMilliseconds) + " ms");

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

    public class TestJob : IJob
    {
        private string _settings;

        public void SetSettings(object settings)
        {
            _settings = settings as string;
        }

        public void Execute(string s) => Console.WriteLine($"{s}:{_settings}");
    }
}
