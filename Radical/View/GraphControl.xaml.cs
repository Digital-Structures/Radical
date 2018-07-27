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
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;

namespace Radical
{
    /// <summary>
    /// Interaction logic for GraphControl.xaml
    /// </summary>
    public partial class GraphControl : UserControl
    {
        public GraphControl()
        {
            this.DataContext = GraphVM;
            InitializeComponent();
        }

        public GraphControl(GraphVM graphVM, RadicalVM radicalVM, RadicalWindow window)
        {
            this.RadicalVM = radicalVM;
            this.GraphVM = graphVM;
            this.DataContext = graphVM;
            this.MyWindow = window;

            InitializeComponent();

            //this.ButtonStats.Style = (Style)window.FindResource("MaterialDesignFloatingActionLightButton");
            //this.ButtonStats.Visibility = Visibility.Collapsed;

            //this.GraphVM.GraphGrid = GraphGrid;
            this.GraphVM.Chart = Chart;
            this.GraphVM.ChartAxisX = ChartAxisX;
            this.GraphVM.ChartAxisY = ChartAxisY;
            this.GraphVM.Window = window; 

          //  this.GraphVM.ChartLine = ChartLine;
            this.GraphVM.ChartLineVisibility(Visibility.Collapsed);
            this.GraphVM.StatisticVisibility = Visibility.Collapsed;

        }

        public GraphVM GraphVM;
        RadicalWindow MyWindow;       
        RadicalVM RadicalVM;

        ////UPDATE WINDOW 
        //public void UpdateWindowGeneral(ChartValues<double> y)
        //{
        //    this.GraphVM.ChartValues = y;
        //}

        //private void Plotter_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    this.GraphVM.SetLineWidth();
        //}

        //CHART MOUSE DOWN
        //Currently returns graph x value of where the mouse clicks down 
        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.GraphVM.ChartValues.Any())
            {
                var mouseCoordinate = e.GetPosition(Chart);
                double chartCoordinateX = Chart.ConvertToChartValues(mouseCoordinate).X;
                int closestXPoint = (int)Chart.Series[0].ClosestPointTo(chartCoordinateX, AxisOrientation.X).X;
                this.GraphVM.UpdateLine(closestXPoint);
                this.RadicalVM.UpdateGraphLines(closestXPoint);
            }
            else
            {
                this.GraphVM.ChartLineVisibility(Visibility.Collapsed);
            }
            //    double mouseX = mouseCoordinate.X;
            //    double minx = ChartAxisX.ActualMinValue;

            //    double ScaleX = (Chart.ActualWidth - 21) / (ChartAxisX.ActualMaxValue - minx);

            //    int actualX = (int)(Math.Truncate(mouseX / ScaleX - minx));
            //    if (actualX < 0)
            //    {
            //        actualX = 0;
            //    }
            //    else if (actualX >= this.GraphVM.ChartValues.Count)
            //    {
            //        actualX = this.GraphVM.ChartValues.Count - 1;
            //    }

            //    this.GraphVM.UpdateLine(actualX);
            //    this.RadicalVM.UpdateGraphLines(actualX);
            //}
            //else
            //{
            //    this.GraphVM.ChartLineVisibility(Visibility.Collapsed);
            //}
        }

        private void ButtonStats_Click(object sender, RoutedEventArgs e)
        {
            if (this.GraphVM.StatisticVisibility == Visibility.Visible)
            {
                this.GraphVM.StatisticVisibility = Visibility.Collapsed;
                this.GraphVM.ButtonStatsIcon = MaterialDesignThemes.Wpf.PackIconKind.Plus;
            }
            else
            {
                this.GraphVM.StatisticVisibility = Visibility.Visible;
                this.GraphVM.ButtonStatsIcon = MaterialDesignThemes.Wpf.PackIconKind.Minus;
            }
        }

        //PREVIEW MOUSE WHEEL
        //Disable scrolling to pan and zoom the chart window
        //This enables the mouse wheel for actually scrolling through the scroll viewer
        private void Graph_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            MyWindow.GraphsScroller.RaiseEvent(e2);
        }
    }
}
