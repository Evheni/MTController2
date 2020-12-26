using MTController2.Exp2;
using MTController2.JobInfo;
using MTController2.MultiThreadingController;
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

namespace WpfAppTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        DispatcherTimer timer = new DispatcherTimer();
       

        QueueBasedController mt;
        public MainWindow()
        {
            InitializeComponent();

            timer.Tick += TimerTick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();

        }

        private async void StartControllerAsync()
        {
            int testQueueSize = 10000;

            Stopwatch stopwatch = new Stopwatch();

            //Init input job queue (which will be processed IN USING THE SAME BEHAVIOR)
            List<IJobInfo> inputQueue = new List<IJobInfo>();
            for (int i = 0; i < testQueueSize; i++)
            {
                inputQueue.Add(new StringJobInfo(i.ToString()));
            }

            //Create controller object passing job process behavior, number of threads to execute jobs, options to init controller
            mt = new LimitedConcurrencyController //new JobWorkerController//
                (
                    new ProcessItemBehaviorJustSleep(new JobProcessorOptions()),//options for particular item
                    50,
                    new ControllerOptions() //options for general controller processor (not for particular item)
                );

            //Fill controller job queue
            mt.FillQueue(inputQueue);

            stopwatch.Start();

            //Launch job execution by controller
            mt.Launch();

            //Wait for all jobs to be finished
            //#? is that ok if we launch it from extra thread
            mt.WaitAllFinished();

            stopwatch.Stop();

            Console.WriteLine($"{mt.GetType()} test: {stopwatch.ElapsedMilliseconds} ms");

        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(StartControllerAsync);
        }

        private async void StopControllerAsync()
        {
            mt.Stop();
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(StopControllerAsync);
        }
        private async void PauseControllerAsync()
        {
            mt.Pause();
        }
        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(PauseControllerAsync);
        }
        private async void ResumeControllerAsync()
        {
            mt.Resume();
        }
        private void ResumButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(ResumeControllerAsync);
        }


        private void TimerTick(object sender, EventArgs e)
        {
            InfoLabel.Content = mt?.ControllerState;
        }
    }
}
