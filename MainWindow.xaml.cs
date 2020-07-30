using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SemaphorePractice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly DispatcherTimer MainTimer = new DispatcherTimer();
        private const int poolSize = 3;
        private readonly SemaphoreSlim pool = new SemaphoreSlim(poolSize, poolSize);
        private readonly ConcurrentQueue<WebsiteDataModel> jobs = new ConcurrentQueue<WebsiteDataModel>();
        public MainWindow()
        {
            InitializeComponent();

            MainTimer.Interval = TimeSpan.FromMilliseconds(250);
            MainTimer.Tick += MainTimer_TickAsync;

        }
        int ticks;
        private async void MainTimer_TickAsync(object sender, EventArgs e)
        {
            if (!jobs.IsEmpty && pool.CurrentCount > 0)
            {
                LogEntry($"Job began on Timer Tick #{++ticks}");
                //LogEntry($"Needing Semaphore. Available: {pool.CurrentCount}  Queue Up Jobs: {items.Count}");
                await pool.WaitAsync();
                LogEntry($"   Acquired Semaphore. Available: {pool.CurrentCount}");
                _ = RunParallelJobsAsync();

            } 
            //while (!jobs.IsEmpty && pool.CurrentCount > 0)
            //{
            //    //LogEntry($"Needing Semaphore. Available: {pool.CurrentCount}  Queue Up Jobs: {items.Count}");
            //    await pool.WaitAsync();
            //    LogEntry($"   Acquired Semaphore. Available: {pool.CurrentCount}");
            //    _ = RunParallelJobsAsync();

            //}

        }

        private void PrepData()
        {
            int i = 0;
            TimeSpan timeSpan = new TimeSpan(0, 0, 5);


            jobs.Enqueue(new WebsiteDataModel
            {
                WebsiteUrl1 = "http://deelay.me/1000/https://www.yahoo.com",
                WebsiteUrl2 = "http://deelay.me/10000/https://www.google.com",
                TimerInterval = timeSpan,
                Instance = ++i
            });
            jobs.Enqueue(new WebsiteDataModel
            {
                WebsiteUrl1 = "http://deelay.me/1000/https://www.microsoft.com",
                WebsiteUrl2 = "http://deelay.me/20000/https://www.cnn.com",
                TimerInterval = timeSpan,
                Instance = ++i
            });
            jobs.Enqueue(new WebsiteDataModel
            {
                WebsiteUrl1 = "http://deelay.me/1000/https://www.codeproject.com",
                WebsiteUrl2 = "http://deelay.me/30000/https://www.stackoverflow.com",
                TimerInterval = timeSpan,
                Instance = ++i
            });
            jobs.Enqueue(new WebsiteDataModel
            {
                WebsiteUrl1 = "http://deelay.me/1000/https://www.yahoo.com",
                WebsiteUrl2 = "http://deelay.me/1000/https://www.google.com",
                TimerInterval = timeSpan,
                Instance = ++i
            });
            jobs.Enqueue(new WebsiteDataModel
            {
                WebsiteUrl1 = "http://deelay.me/1000/https://www.microsoft.com",
                WebsiteUrl2 = "http://deelay.me/1000/https://www.cnn.com",
                TimerInterval = timeSpan,
                Instance = ++i
            });
            jobs.Enqueue(new WebsiteDataModel
            {
                WebsiteUrl1 = "http://deelay.me/1000/https://www.codeproject.com",
                WebsiteUrl2 = "http://deelay.me/1000/https://www.stackoverflow.com",
                TimerInterval = timeSpan,
                Instance = ++i
            });
            jobs.Enqueue(new WebsiteDataModel
            {
                WebsiteUrl1 = "http://deelay.me/1000/https://www.yahoo.com",
                WebsiteUrl2 = "http://deelay.me/1000/https://www.google.com",
                TimerInterval = timeSpan,
                Instance = ++i
            });
            jobs.Enqueue(new WebsiteDataModel
            {
                WebsiteUrl1 = "http://deelay.me/1000/https://www.microsoft.com",
                WebsiteUrl2 = "http://deelay.me/1000/https://www.cnn.com",
                TimerInterval = timeSpan,
                Instance = ++i
            });
            jobs.Enqueue(new WebsiteDataModel
            {
                WebsiteUrl1 = "http://deelay.me/1000/https://www.codeproject.com",
                WebsiteUrl2 = "http://deelay.me/1000/https://www.stackoverflow.com",
                TimerInterval = timeSpan,
                Instance = ++i
            });

        }
        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            // SemaphoreSlim pool = new SemaphoreSlim(poolSize, poolSize);
            CheckBox checkbox = (CheckBox)sender;
            bool status = checkbox.IsChecked ?? false;
            if (status)
            {
                LogEntry(">>>>    Checked Event    <<<<");

                PrepData();
                MainTimer.Start();
            }
            else
            {
                LogEntry("<<<<    Unchecked Event    >>>>");
                while (jobs.TryDequeue(out _)) ;
                MainTimer.Stop();
            }
        }

        private async Task RunParallelJobsAsync()
        {

            //WebsiteDataModel output = new WebsiteDataModel();
            jobs.TryDequeue(out WebsiteDataModel output);
            if (output != null)
            {
                await DownloadWebsiteAsync(output);

            }
            pool.Release();
            LogEntry($"                      Semaphore Released.  Available: {pool.CurrentCount}.  Queue Up Jobs: {jobs.Count}");
        }

        private async Task<WebsiteDataModel> DownloadWebsiteAsync(WebsiteDataModel wdm)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            WebClient client = new WebClient();

            //Task.Delay(wdm.TimerInterval).Wait();

            LogEntry($"*        {wdm.WebsiteUrl1} Downloading.   **{wdm.Instance}");
            wdm.WebsiteData1 = await client.DownloadStringTaskAsync(wdm.WebsiteUrl1);

            double Lap1 = stopWatch.Elapsed.TotalSeconds;
            LogEntry($"*        {wdm.WebsiteUrl1} Downloaded. {wdm.WebsiteData1.Length} bytes in {Lap1:0.00}secs.  *{wdm.Instance}");

            LogEntry($">>               {wdm.WebsiteUrl2} Downloading.    @@{wdm.Instance}");
            wdm.WebsiteData2 = await client.DownloadStringTaskAsync(wdm.WebsiteUrl2);

            //Task.Delay(wdm.TimerInterval).Wait();

            double Lap2 = stopWatch.Elapsed.TotalSeconds - Lap1;
            LogEntry($">>               {wdm.WebsiteUrl2} Downloaded. {wdm.WebsiteData2.Length} bytes in {Lap2:0.00}secs.    >{wdm.Instance}");

            jobs.Enqueue(wdm);
            return wdm;
        }

        private void LogEntry(string text)
        {
            //  this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => txtBox.Text += $"{text}{Environment.NewLine}"));
            txtBox.Text += $"{text}{Environment.NewLine}";
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            txtBox.Text = string.Empty;
        }
    }
}
