using System;
using System.Collections.Generic;
using System.Text;

namespace MTController2.JobInfo
{
    public class StringJobInfo : IJobInfo
    {
        private string _job;

        public string Job
        {
            get { return _job; }
            set { _job = value; }
        }

        public StringJobInfo(string job)
        {
            _job = job;
        }

        //public void Process()
        //{

        //}
    }
}
