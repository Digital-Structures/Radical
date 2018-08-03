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
using DSOptimization;

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

        private ChartValues<LiveCharts.Defaults.ObservablePoint> _hovervalue;
        public ChartValues<LiveCharts.Defaults.ObservablePoint> HoverValue
        {
            get { return _hovervalue; }
            set
            {
                if (CheckPropertyChanged<ChartValues<LiveCharts.Defaults.ObservablePoint>>("HoverValue", ref _hovervalue, ref value))
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

        private int _iteration;
        public int Iteration
        {
            get { return _iteration; }
            set
            {
                if (CheckPropertyChanged<int>("Iteration", ref _iteration, ref value))
                {
                    LiveCharts.Defaults.ObservablePoint point = new LiveCharts.Defaults.ObservablePoint();
                    point.X = Iteration;
                    point.Y = ChartValues.ElementAt(Iteration);

                    HoverValue = new ChartValues<LiveCharts.Defaults.ObservablePoint> { point };
                }
            }
        }

        public LiveCharts.Wpf.LineSeries _plotter;
        public LiveCharts.Wpf.LineSeries Plotter
        {
            get { return _plotter; }
            set
            {
                _plotter = value;
            }
        }

        public RadicalWindow _window;
        public RadicalWindow Window
        {
            get { return _window; }
            set
            {
                _window = value;
            }
        }

        public double _graphgridheight;
        public double GraphGridHeight
        {
            get { return _graphgridheight; }
            set
            {
                _graphgridheight = value;
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

        //THIS TOOL SHOULD BE IMPROVED
        private bool _optimizerdone;
        public bool OptimizerDone
        {
            get { return _optimizerdone; }
            set
            {
                _optimizerdone = value;
                if (value)
                {
                    ButtonStatsVisibility = Visibility.Visible;
                    ButtonStatsIcon = MaterialDesignThemes.Wpf.PackIconKind.Plus;
                }
                else
                {
                    ButtonStatsVisibility = Visibility.Collapsed;
                    //ButtonStatsIcon = MaterialDesignThemes.Wpf.PackIconKind.Minus;
                }
            }
        }


        private MaterialDesignThemes.Wpf.PackIconKind _buttonstatsicon;
        public MaterialDesignThemes.Wpf.PackIconKind ButtonStatsIcon
        {
            get { return _buttonstatsicon; }
            set
            {
                if (CheckPropertyChanged<MaterialDesignThemes.Wpf.PackIconKind>("ButtonStatsIcon", ref _buttonstatsicon, ref value))
                {
                }
            }
        }

        private Visibility _buttonstatsvisibility;
        public Visibility ButtonStatsVisibility
        {
            get { return _buttonstatsvisibility; }
            set
            {
                if (CheckPropertyChanged<Visibility>("ButtonStatsVisibility", ref _buttonstatsvisibility, ref value))
                {
                }
            }
        }

        public void UpdateLine(int iteration)
        {
            this.DisplayX = iteration.ToString();

            //Make sure OptimizerDone is actually functioning its purpose
            if (ChartValues.Any() && OptimizerDone)
            {
                ChartLineVisibility(Visibility.Visible);

                double yValue = ChartValues.ElementAt(iteration);
                this.DisplayY = String.Format("{0:0.000}",yValue);

                Iteration = iteration;
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

        public void ChartLineVisibility(Visibility v)
        {
            ChartLineVisibilityX = v;
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

        private double _chartliney2;
        public double ChartLineY2
        {
            get { return _chartliney2; }
            set
            {
                if(CheckPropertyChanged<double>("ChartLineY2", ref _chartliney2, ref value))
                {
                }
            }
        }

        private double _finaloptimizedvalue;
        public double FinalOptimizedValue
        {
            get { return _finaloptimizedvalue; }
            set
            {
                if(CheckPropertyChanged<double>("FinalOptimizedValue", ref _finaloptimizedvalue, ref value))
                {
                    FinalOptimizedValueString = String.Format("{0:0.000000}", FinalOptimizedValue);
                }
            }
        }

        private string _finaloptimizedvaluestring;
        public string FinalOptimizedValueString
        {
            get { return _finaloptimizedvaluestring; }
            set
            {
                if (CheckPropertyChanged<string>("FinalOptimizedValueString", ref _finaloptimizedvaluestring, ref value))
                {
                }
            }
        }



        private string _trackedvaluetext;
        public string TrackedValueText
        {
            get { return "tracked value " + _trackedvaluetext; }
            set
            {
                if (CheckPropertyChanged<string>("TrackedValueText", ref _trackedvaluetext, ref value))
                {
                }
            }
        }

        public void CalculateChartLineY2()
        {
            ChartLineY2 = GraphGridHeight - 214;
        }

        //I don't think this is actually being used
        public void SetLineWidth()
        {
            this.ChartLineWidth = this.Chart.ActualWidth + 25;
        }


        public void UpdateHeight()
        {
            GraphGridHeight = Window.MainGrid.ActualHeight * 0.45;
        }

        //Not sure if this is actually being used
        private Visibility _chartrowvisibility; 
        public Visibility ChartRowVisibility
        {
            get { return _chartrowvisibility; }
            set
            {
                if (CheckPropertyChanged<Visibility>("ChartRowVisibility", ref _chartrowvisibility, ref value))
                {
                }
            }
        }
    }
}
