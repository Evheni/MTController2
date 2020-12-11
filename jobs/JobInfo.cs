using System;

namespace Core.Jobs
{
    public class JobInfo
    {
        public long   JobId { get; set; }
        public string Name  { get; set; }
        
        public Type      JobClass     { get; set; }
        public object    Settings     { get; set; }
        public TimeSpan  TimeWork     =>TimeEnd -TimeStart ;
        public DateTime  TimeStart    { get; set; }
        public DateTime  TimeEnd      { get; set; }
        public JobResult JobResult    { get; set; }
        public string    ErrorMessage { get; set; }
        public object    Result       { get; set; }
    }
}