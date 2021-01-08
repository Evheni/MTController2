using MTController2.Exp2;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Jobs;
using MTController2.JobInfo;
using MTController2.MultiThreadingController;
using MTController2.OptionClasses;
using System.Diagnostics;

namespace TestAppConsole
{
    class Program
    {
        static void Main(string[] args)
        {
          
            int testQueueSize = 10000;

            Stopwatch stopwatch = new Stopwatch();

            //Init input job queue (which will be processed IN USING THE SAME BEHAVIOR)
            List<IJobInfo> inputQueue = new List<IJobInfo>();
            for (int i = 0; i < testQueueSize; i++)
            {
                inputQueue.Add(new StringJobInfo(i.ToString()));
            }

            //Create controller object passing job process behavior, number of threads to execute jobs, options to init controller
            Controller mt = new LimitedConcurrencyController //new JobWorkerController//
                (
                    new JobProcessBehaviorJustSleep(new JobProcessorOptions()),//options for particular item
                    50,
                    new ControllerOptions() //options for general controller processor (not for particular item)
                );

            //Fill controller job queue
            mt.FillQueue(inputQueue);

            stopwatch.Start();

            //Launch job execution by controller
            mt.Launch();

            //Wait for all jobs to be finished
            //#? is that ok if we launch it from extra thread
            mt.WaitAllFinished();

            stopwatch.Stop();
            
            Console.WriteLine($"{mt.GetType()} test: {stopwatch.ElapsedMilliseconds} ms");

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

}


