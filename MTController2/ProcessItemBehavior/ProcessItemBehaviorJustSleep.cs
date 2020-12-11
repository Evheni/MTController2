using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MTController2.Exp2
{
    class ProcessItemBehaviorJustSleep : IProcessItemBehavior<string>
    {
        public void Process(string job)
        {
            
            Thread.Sleep(500);
            Console.WriteLine($"job {job} after sleep");

        }
    }
}
