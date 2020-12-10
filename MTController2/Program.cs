using MTController2.Exp2;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MTController2
{
    class Program
    {
        static void Main(string[] args)
        {
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
}
