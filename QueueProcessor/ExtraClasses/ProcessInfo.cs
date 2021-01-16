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

        private int _elementsInQueue;

        public int ElementsInQueue
        {
            get { return _elementsInQueue; }
            set { 
                _elementsInQueue = value;
                OnPropertyChanged();
            }
        }

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