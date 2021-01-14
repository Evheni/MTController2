using System;
using System.Collections.Generic;
using System.Text;

namespace MTController2.ExtraClasses
{

    public class AllFinishedEventArgs : EventArgs
    {
        public bool IsSuccess { get; private set; }
        public Exception Error { get; private set; }
        public AllFinishedEventArgs(bool isSuccess, Exception exp = null)
        {
            IsSuccess = isSuccess;
            Error = exp;
        }
    }
}
