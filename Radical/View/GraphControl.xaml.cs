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

            this.GraphVM.Plotter = Plotter;
            this.Axis.Ticks = new List<double>{ 20 };
            this.GraphVM.ChartLine = ChartLine;
            this.GraphVM.ChartLineVisibility(Visibility.Collapsed);  
        }
        public GraphVM GraphVM;
        RadicalWindow MyWindow;       
        RadicalVM RadicalVM;

        //UPDATE WINDOW 
        public void UpdateWindowGeneral(IEnumerable<double> y)
        {
            var x = Enumerable.Range(0, y.Count()).ToArray();

            Dispatcher.Invoke(() =>
            {
                Plotter.Plot(x, y);
            });
        }

        //SIZE CHANGED
        public void Plotter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.GraphVM.SetLineWidth();
        }

        //CHART MOUSE DOWN
        //Currently returns graph x value of where the mouse clicks down 
        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.GraphVM.GraphScores.Any())
            {
                double mouseX = e.GetPosition(this.Plotter).X + Plotter.OffsetX;
                double ScaleX = Plotter.ScaleX;

                int actualX = (int)(Math.Truncate(mouseX / ScaleX));
                if (actualX < 0)
                {
                    actualX = 0;
                }
                else if (actualX >= this.GraphVM.GraphScores.Count)
                {
                    actualX = this.GraphVM.GraphScores.Count - 1;
                }

                this.GraphVM.UpdateLine(actualX);
                this.RadicalVM.UpdateGraphLines(actualX);
            }
            else
            {
                this.GraphVM.ChartLineVisibility(Visibility.Collapsed);
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

        private void Plotter_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
