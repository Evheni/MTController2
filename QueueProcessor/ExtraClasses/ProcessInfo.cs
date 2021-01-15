using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MTController2.MultiThreadingController
{
    public class ProcessInfo : INotifyPropertyChanged
    {
        private int _results;

        public int Results
        {
            get { return _results; }
            set 
            { 
                _results = value;
                OnPropertyChanged();
            }
        }

        public int ElementsInQueue;
        public int QueueElementProcessed;

        public ProcessInfo()
        {
            ElementsInQueue = 0;
            QueueElementProcessed = 0;
            Results = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}