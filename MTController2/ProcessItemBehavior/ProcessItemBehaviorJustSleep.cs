using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    class ProcessItemBehaviorJustSleep : IProcessItemBehavior<string>
    {
        public void Process(string job) 
        {
            //await Task.Delay(10);
            Thread.Sleep(10);
            Console.WriteLine($"job {job} after sleep");

        }
    }
}
