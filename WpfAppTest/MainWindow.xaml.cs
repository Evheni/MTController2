using MTController2.Exp2;
using MTController2.JobInfo;
using MTController2.MultiThreadingController;
using MTController2.OptionClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
       
        Controller mt;
        public MainWindow()
        {
            InitializeComponent();

            //Create controller object passing job process behavior, number of threads to execute jobs, options to init controller
            mt = new LimitedConcurrencyController //new JobWorkerController//
                (
                    new JobProcessBehaviorJustSleep(new JobProcessorOptions()),//options for particular item
                    50,
                    new ControllerOptions() //options for general controller processor (not for particular item)
                );
            // mt.AllFinished += Mt_AllFinished;

            timer.Tick += TimerTick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();

        }

        private void Mt_AllFinished(object sender, AllFinishedEventArgs e)
        {
            // UpdateStateLabel();
        }

        private async void StartControllerAsync()
        {
            try
            {
                int testQueueSize = 20000;

                mt.Init();

                //Init input job queue (which will be processed IN USING THE SAME BEHAVIOR)
                List<IJobInfo> inputQueue = new List<IJobInfo>();
                for (int i = 0; i < testQueueSize; i++)
                {
                    inputQueue.Add(new StringJobInfo(i.ToString()));
                }
                Debug.WriteLine("Start filling queue to controller");
                //Fill controller job queue
                mt.FillQueue(inputQueue);

                //Launch job execution by controller
                mt.Launch();

                // UpdateStateLabel();

                mt.WaitAllFinished();
                // UpdateStateLabel();
            }
            catch (Exception exp)
            {
                // UpdateStateLabel(exp.Message);
            }
                
           
            
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Task t = Task.Factory.StartNew(StartControllerAsync);

        }

        //private async void StopControllerAsync()
        //{
        //    mt.Stop();
        //    // UpdateStateLabel();
        //}
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            mt.Stop();
            // Task.Factory.StartNew(StopControllerAsync);
        }
      
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mt.Pause();
        }
       
        private void ResumButton_Click(object sender, RoutedEventArgs e)
        {
            mt.Resume();
        }


        private void TimerTick(object sender, EventArgs e)
        {
            InfoLabel.Content = mt?.ControllerState;
            ResultLabel.Content = "Elements processed: " + mt?.ProcessInfo?.Results;
            QueueLabel.Content = "Elements in queue: " + mt?.ProcessInfo?.ElementsInQueue;
        }

        //private void UpdateStateLabel(string str="")
        //{

        //    Dispatcher.Invoke(new Action(() => {
        //        InfoLabel.Content = (str=="") ? mt.ControllerState: str ;
        //    }));
        //}
    }
}
