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

           // _graphVisibility = Visibility.Visible;
           // DisplayX = null;
           // DisplayY = null;
           // _chartanimationsdisabled = false;
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
       
        public Grid _graphgrid; 
        //public Grid GraphGrid
        //{
        //    get { return _graphgrid; }
        //    set
        //    {
        //        _graphgrid = value; 
        //    }
        //}

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

        private Line _chartline;
        public Line ChartLine
        {
            get { return _chartline; }
            set { _chartline = value; }
        }

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
                this.DisplayY = String.Format("{0:0.00}",yValue);

                Iteration = iteration;

                TrackedValueText = String.Format("{0:0.00}", ChartValues.ElementAt(Iteration));

                //double minX = ChartAxisX.ActualMinValue;
                //double ScaleX = (Chart.ActualWidth - 21) / (ChartAxisX.ActualMaxValue - minX);

                ////Calculations for the horizontal line
                ////actualX * ScaleX scales the graph value to appropriate mouse position
                ////+45 is a hardcoded value because the position is off due to the side of the graph
                ////minx takes into account if the graph has been moved 
                //double newXPosition = iteration * ScaleX - minX + 25;
                
                //if (newXPosition - 25 < 0 || newXPosition > Chart.ActualWidth)
                //{
                //    ChartLineVisibilityX = Visibility.Collapsed;
                //}
                //this.ChartLineX = newXPosition;

                //Calculatoins for the vertical line
                //double minY = ChartAxisY.ActualMinValue;
                //double ScaleY = (Chart.ActualHeight) / (ChartAxisY.ActualMaxValue - minY);
                //double newYPosition = Chart.ActualHeight - (yValue * ScaleY + minY);
                //if (newYPosition < 0 || newYPosition > Chart.ActualHeight)
                //{
                //    ChartLineVisibilityY = Visibility.Collapsed;
                //}
                //this.ChartLineY = newYPosition;
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

        //private Visibility _chartlinevisibilityy;
        //public Visibility ChartLineVisibilityY
        //{
        //    get { return _chartlinevisibilityy; }
        //    set
        //    {
        //        if (CheckPropertyChanged<Visibility>("ChartLineVisibilityY", ref _chartlinevisibilityy, ref value))
        //        {
        //        }
        //    }
        //}

        public void ChartLineVisibility(Visibility v)
        {
            ChartLineVisibilityX = v;
            //ChartLineVisibilityY = v;
            
            //if (ChartLineX - 25 < 0 || ChartLineX > this.Chart.ActualWidth + 25)
            //{
            //    ChartLineVisibilityX = Visibility.Collapsed;
            //    DisplayX = null;
            //    DisplayY = null;
            //}
            //if (ChartLineY < 0 || ChartLineY > this.Chart.ActualHeight)
            //{
            //    ChartLineVisibilityY = Visibility.Collapsed;
            //    DisplayY = null;
            //}
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

        private string _finaloptimizedvalue;
        public string FinalOptimizedValue
        {
            get { return _finaloptimizedvalue; }
            set
            {
                if(CheckPropertyChanged<string>("FinalOptimizedValue", ref _finaloptimizedvalue, ref value))
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

        private string _totalimprovementtext;
        public string TotalImprovementText
        {
            get { return "total improvement " + _totalimprovementtext + "%"; }
            set
            {
                if(CheckPropertyChanged<string>("TotalImprovementText", ref _totalimprovementtext, ref value))
                {
                }
            }
        }

        private string _convergenceratetext;
        public string ConvergeneceRateText
        {
            get { return "convergence rate " + _convergenceratetext + "%"; }
            set
            {
                if (CheckPropertyChanged<string>("ConvergeneceRateText", ref _convergenceratetext, ref value))
                {
                }
            }
        }

        public void Statistics()
        {
            double optimizedValue = ChartValues.ElementAt(ChartValues.Count - 1);
            FinalOptimizedValue = String.Format("{0:0.00}", optimizedValue);

            double improvement = optimizedValue / ChartValues.ElementAt(0) * 100;
            if (improvement > 100)
            {
                improvement = 0; 
            }
            TotalImprovementText = String.Format("{0:0}", improvement);

            //How do you find convergence?
            double convergence = 100;
            ConvergeneceRateText = String.Format("{0:0.00}", convergence);
        }

        private Visibility _statisticvisibility;
        public Visibility StatisticVisibility
        {
            get { return _statisticvisibility; }
            set
            {
                if(CheckPropertyChanged<Visibility>("StatisticVisibility", ref _statisticvisibility, ref value))
                {
                }
            }
        }

        public void CalculateChartLineY2()
        {
            ChartLineY2 = GraphGridHeight - 214;
        }

        public void SetLineWidth()
        {
            this.ChartLineWidth = this.Chart.ActualWidth + 25;
        }

        //public void UpdateGraphWidth()
        //{
        //    //this.Chart.Width = GraphGrid.ActualWidth;
        //}

        public void UpdateHeight()
        {
            GraphGridHeight = Window.MainGrid.ActualHeight * 0.45;
        }

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
