namespace MTController2.MultiThreadingController
{
    public class ProcessInfo
    {
        public int ElementsInQueue;
        public int QueueElementProcessed;
        public int Results;

        public ProcessInfo()
        {
            ElementsInQueue = 0;
            QueueElementProcessed = 0;
            Results = 0;
        }
    }
}