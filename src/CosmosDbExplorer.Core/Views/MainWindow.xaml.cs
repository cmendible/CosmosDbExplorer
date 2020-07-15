using System;
using System.Collections.Generic;
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
using AutoUpdaterDotNET;
using CosmosDbExplorer.Properties;
using CosmosDbExplorer.ViewModels;
using Newtonsoft.Json;

namespace CosmosDbExplorer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupAutoUpdater();
        }

        private void SetupAutoUpdater()
        {
            // Allow user to be reminded to update in 1 day
            AutoUpdater.ShowRemindLaterButton = true;
            AutoUpdater.LetUserSelectRemindLater = false;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Days;
            AutoUpdater.RemindLaterAt = 1;

            AutoUpdater.ReportErrors = false;
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.DownloadPath = Environment.CurrentDirectory;
            AutoUpdater.ParseUpdateInfoEvent += AutoUpdateOnParseUpdateInfoEvent;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Settings.Default.AutoUpdaterIntervalInSeconds) };
            timer.Tick += delegate
            {
                AutoUpdater.Start(Settings.Default.AutoUpdaterUrl);
            };
            timer.Start();
        }

        private void AutoUpdateOnParseUpdateInfoEvent(ParseUpdateInfoEventArgs args)
        {
            // Use JSON format for AutoUpdate release inforrmatin file
            dynamic json = JsonConvert.DeserializeObject(args.RemoteData);
            args.UpdateInfo = new UpdateInfoEventArgs
            {
                CurrentVersion = json.version,
                ChangelogURL = json.changelog,
                Mandatory = new Mandatory { Value = json.mandatory },
                DownloadURL = json.url,
                CheckSum = json.checksum
            };
        }

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;
            vm.RequestClose += () => Close();

            AutoUpdater.Start(Settings.Default.AutoUpdaterUrl);
        }
    }
}
