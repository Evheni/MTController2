using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MTController2.Exp2
{
    class ProcessItemBehaviorForSimpleUrl : IProcessItemBehavior<string>
    {
        public void Process(string job)
        {
            Console.WriteLine(job);

        }
    }
}
