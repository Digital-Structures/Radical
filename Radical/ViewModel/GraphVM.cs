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
using LiveCharts.Wpf;


namespace Radical
{
    public class GraphVM : BaseVM
    {
        public GraphVM(ChartValues<double> scores, string name)
        {
            _chartvalues = scores; 
            _linegraph_name = name;
            _chartlinex = 0;
            _x = "0";
            _y = "0";
            _graphVisibility = Visibility.Visible;
            DisplayX = null;
            DisplayY = null;
            _chartvalues = new ChartValues<double>();
            _chartanimationsdisabled = false;
        }

        //This is the specific Chart Values array, it seems that you cannot pass in an ordinary list 
        private ChartValues<double> _chartvalues;
        public ChartValues<double> ChartValues
        {
            get { return _chartvalues; }
            set
            {
                if (CheckPropertyChanged<ChartValues<double>>("ChartValues", ref _chartvalues, ref value))
                {
                }
            }
        }

        private String _linegraph_name;
        public String LineGraphName
        {
            get
            { return _linegraph_name;}
            set
            {
                if (CheckPropertyChanged<String>("LineGraphName", ref _linegraph_name, ref value))
                {
                }
            }
        }

        private int _chartlineiteration;
        public int ChartLineIteration
        {
            get
            {
                return _chartlineiteration;
            }
            set
            {
                _chartlineiteration = value;
            }
        }

        private double _chartlinex;
        public double ChartLineX
        {
            get { return _chartlinex; }
            set
            {
                if (CheckPropertyChanged<double>("ChartLineX", ref _chartlinex, ref value))
                {
                }
            }
        }

        private double _chartliney;
        public double ChartLineY
        {
            get { return _chartliney; }
            set
            {
                if (CheckPropertyChanged<double>("ChartLineY", ref _chartliney, ref value))
                {
                }
            }
        }

        private string _x;
        public string DisplayX
        {
            get { return String.Format("Iteration: {0}", _x); }
            set
            {
                CheckPropertyChanged<string>("DisplayX", ref _x, ref value);
            }
        }

        private string _y;
        public string DisplayY
        {
            get { return String.Format("{0}: {1}", this.LineGraphName, _y); }
            set
            {
                CheckPropertyChanged<string>("DisplayY", ref _y, ref value);
            }
        }

        public LiveCharts.Wpf.CartesianChart _chart;
        public LiveCharts.Wpf.CartesianChart Chart
        {
            get { return _chart; }
            set
            {
                _chart = value;
            }
        }

        public LiveCharts.Wpf.Axis _chartaxisx;
        public LiveCharts.Wpf.Axis ChartAxisX
        {
            get { return _chartaxisx; }
            set
            {
                _chartaxisx = value;
            }
        }

        public LiveCharts.Wpf.Axis _chartaxisy;
        public LiveCharts.Wpf.Axis ChartAxisY
        {
            get { return _chartaxisy; }
            set
            {
                _chartaxisy = value;
            }
        }

        private Line _chartline;
        public Line ChartLine
        {
            get { return _chartline; }
            set { _chartline = value; }
        }

        private bool _showline;
        public bool ShowLine
        {
            get { return _showline; }
            set { _showline = value; }
        }

        public void UpdateLine(int iteration)
        {
            this.DisplayX = iteration.ToString();

            if (ChartValues.Any() && ShowLine)
            {
                ChartLineVisibility(Visibility.Visible);
                double yValue = ChartValues.ElementAt(iteration);
                this.DisplayY = String.Format("{0:0.00}",yValue);

                double minX = ChartAxisX.ActualMinValue;
                double ScaleX = (Chart.ActualWidth) / (ChartAxisX.ActualMaxValue - minX);

                //Calculations for the horizontal line
                //actualX * ScaleX scales the graph value to appropriate mouse position
                //+45 is a hardcoded value because the position is off due to the side of the graph
                //minx takes into account if the graph has been moved 
                double newXPosition = iteration * ScaleX - minX + 25;
                
                if (newXPosition - 25 < 0 || newXPosition > Chart.ActualWidth)
                {
                    ChartLineVisibilityX = Visibility.Collapsed;
                }
                this.ChartLineX = newXPosition;

                //Calculatoins for the vertical line
                double minY = ChartAxisY.ActualMinValue;
                double ScaleY = (Chart.ActualHeight) / (ChartAxisY.ActualMaxValue - minY);
                double newYPosition = Chart.ActualHeight - (yValue * ScaleY + minY);
                if (newYPosition < 0 || newYPosition > Chart.ActualHeight)
                {
                    ChartLineVisibilityY = Visibility.Collapsed;
                }
                this.ChartLineY = newYPosition;
            }
        }

        //GRAPH VISIBILITY
        //Disables graph visibility when you don't want to see it (checkbox option)
        private Visibility _graphVisibility;
        public Visibility GraphVisibility
        {
            get
            {
                return _graphVisibility;
            }
            set
            {
                CheckPropertyChanged<Visibility>("GraphVisibility", ref _graphVisibility, ref value);
            }
        }

        private Visibility _chartlinevisiblityx;
        public Visibility ChartLineVisibilityX
        {
            get { return _chartlinevisiblityx; }
            set
            {
                if (CheckPropertyChanged<Visibility>("ChartLineVisibilityX", ref _chartlinevisiblityx, ref value))
                {
                }
            }
        }

        private Visibility _chartlinevisibilityy;
        public Visibility ChartLineVisibilityY
        {
            get { return _chartlinevisibilityy; }
            set
            {
                if (CheckPropertyChanged<Visibility>("ChartLineVisibilityY", ref _chartlinevisibilityy, ref value))
                {
                }
            }
        }

        public void ChartLineVisibility(Visibility v)
        {
            ChartLineVisibilityX = v;
            ChartLineVisibilityY = v;
            
            if (ChartLineX - 25 < 0 || ChartLineX > this.Chart.ActualWidth + 25)
            {
                ChartLineVisibilityX = Visibility.Collapsed;
                DisplayX = null;
                DisplayY = null;
            }
            if (ChartLineY < 0 || ChartLineY > this.Chart.ActualHeight)
            {
                ChartLineVisibilityY = Visibility.Collapsed;
                DisplayY = null;
            }
        }

        private double _chartlinewidth;
        public double ChartLineWidth
        {
            get { return _chartlinewidth; }
            set
            {
                if (CheckPropertyChanged<double>("ChartLineWidth", ref _chartlinewidth, ref value))
                {
                }
            }
        }

        public void SetLineWidth()
        {
            this.ChartLineWidth = this.Chart.ActualWidth + 25;
        }

        private bool _chartanimationsdisabled;
        public bool ChartAnimationsDisabled
        {
            get { return _chartanimationsdisabled; }
            set
            {
                if (CheckPropertyChanged<bool>("ChartAnimationsDisabled", ref _chartanimationsdisabled, ref value))
                {
                }
            }
        }

    }
}
