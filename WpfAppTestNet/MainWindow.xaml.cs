using MTController2.Exp2;
using MTController2.JobInfo;
using MTController2.OptionClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace WpfAppTestNet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int testQueueSize;
        int threadNumber;
        int sleepDuration;

        JobProcessorOptions jobProcessorOptions;

        DispatcherTimer timer = new DispatcherTimer();

        Controller mt;

        void InitTest()
        {
            int mode = 3;
            switch (mode)
            {
                case 0:
                    testQueueSize = 20000;
                    threadNumber = 10;
                    sleepDuration = 1000;
                    break;
                case 1:
                    testQueueSize = 20000;
                    threadNumber = 1;
                    sleepDuration = 1000;
                    break;
                case 2:
                    testQueueSize = 20000;
                    threadNumber = 2;
                    sleepDuration = 1000;
                    break;
                case 3:
                    testQueueSize = 20000;
                    threadNumber = 50;
                    sleepDuration = 1000;
                    break;
                default:
                    break;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            InitTest();

            jobProcessorOptions = new JobProcessorOptions(sleepDuration);


            //Create controller object passing job process behavior, number of threads to execute jobs, options to init controller
            mt = new LimitedConcurrencyController //new JobWorkerController//
                (
                    new JobProcessBehaviorJustSleep(jobProcessorOptions),//options for particular item
                    threadNumber,
                    new ControllerOptions() //options for general controller processor (not for particular item)
                );

            DataContext = mt;

            timer.Tick += TimerTick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();

        }


        private async void StartControllerAsync()
        {
            try
            {
                //mt = new LimitedConcurrencyController //new JobWorkerController//
                //(
                //    new JobProcessBehaviorJustSleep(jobProcessorOptions),//options for particular item
                //    threadNumber,
                //    new ControllerOptions() //options for general controller processor (not for particular item)
                //);
                // DataContext = mt;
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
                Debug.WriteLine(exp.Message);
                // UpdateStateLabel(exp.Message);
            }



        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Task t = Task.Factory.StartNew(StartControllerAsync);

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mt.Stop();
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }

            // Task.Factory.StartNew(StopControllerAsync);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mt.Pause();
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }
        }

        private void ResumButton_Click(object sender, RoutedEventArgs e)
        {
            //mt.ProcessInfo.Results = 67;

            try
            {
                mt.Resume();
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }
        }


        private void TimerTick(object sender, EventArgs e)
        {
            InfoLabel.Content = mt?.ControllerState;
            // ResultLabel.Text = "Elements processed: " + mt?.ProcessInfo?.Results;
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