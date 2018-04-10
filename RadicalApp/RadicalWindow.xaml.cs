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
using System.ComponentModel;

namespace RadicalApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RadicalWindow : Window
    {
        public RadicalWindow()
        {
            //this.DataContext = WindowVM;
            InitializeComponent();
        }

        //private RadicalVM WindowVM { get; set; }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void OpenOptSettings(object sender, RoutedEventArgs e)
        {

        }

        private void Export_SVG(object sender, RoutedEventArgs e)
        {

        }

        private void Export_CSV(object sender, RoutedEventArgs e)
        {

        }

        bool isWorking = false;


        private async void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            isWorking = true;

            ButtonPause.Visibility = Visibility.Visible;
            ButtonPlay.Visibility = Visibility.Collapsed;
            await Task.Run(() => DrawGraph(1001));

        }
        
        void DrawGraph(int n)
        {
            int i = 2;
            while (isWorking && i<=n)
            {
                var x = Enumerable.Range(0, i).Select(j => j / 10.0).ToArray();
                var y = x.Select(v => Math.Abs(v) < 1e-10 ? 1 : Math.Sin(v) / v).ToArray();
                UpdateWindow(x, y);
                System.Threading.Thread.Sleep(100);
                i++;
            }
        }

        void UpdateWindow(IEnumerable<double> x, IEnumerable<double> y)
        {
            //safe call
            Dispatcher.Invoke(() =>
            {
                Plotter.Plot(x, y);
            });
        }

            private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            isWorking = false;
            ButtonPause.Visibility = Visibility.Collapsed;
            ButtonPlay.Visibility = Visibility.Visible;
            //BackgroundWorker worker = new BackgroundWorker();
            //worker.WorkerReportsProgress = true;
            //worker.DoWork += worker_DoWork;
            //worker.ProgressChanged += worker_ProgressChanged;
            //worker.RunWorkerAsync();

        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
        }
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        }

    }

    public class VisibilityToCheckedConverter : IValueConverter

    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {

            return ((Visibility)value) == Visibility.Visible;

        }



        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {

            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;

        }

    }
}
