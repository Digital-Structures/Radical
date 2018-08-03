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
            //_chartlinex = 0;
            //_x = "0";
            _y = "0";
            _graphaxislabelsvisibility = Visibility.Hidden;
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

        //private double _chartlinex;
        //public double ChartLineX
        //{
        //    get { return _chartlinex; }
        //    set
        //    {
        //        if (CheckPropertyChanged<double>("ChartLineX", ref _chartlinex, ref value))
        //        {
        //        }
        //    }
        //}

        //private double _chartliney;
        //public double ChartLineY
        //{
        //    get { return _chartliney; }
        //    set
        //    {
        //        if (CheckPropertyChanged<double>("ChartLineY", ref _chartliney, ref value))
        //        {
        //        }
        //    }
        //}

        //private string _x;
        //public string DisplayX
        //{
        //    get { return String.Format("Iteration: {0}", _x); }
        //    set
        //    {
        //        CheckPropertyChanged<string>("DisplayX", ref _x, ref value);
        //    }
        //}

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

        private string _y;
        public string DisplayY
        {
            get { return String.Format("{0}: {1}", this.LineGraphName, _y); }
            set
            {
                CheckPropertyChanged<string>("DisplayY", ref _y, ref value);
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
            }
        }

        public void UpdateLine(int iteration)
        {
            //this.DisplayX = iteration.ToString();

            //Make sure OptimizerDone is actually functioning its purpose
            if (ChartValues.Any() && OptimizerDone)
            {
                ChartLineVisibility = Visibility.Visible;

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

        private Visibility _chartlinevisiblity;
        public Visibility ChartLineVisibility
        {
            get { return _chartlinevisiblity; }
            set
            {
                if (CheckPropertyChanged<Visibility>("ChartLineVisibility", ref _chartlinevisiblity, ref value))
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

        //There has to be a better way
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

        public void UpdateHeight()
        {
            GraphGridHeight = Window.MainGrid.ActualHeight * 0.45;
        }

        private Visibility _graphaxislabelsvisibility;
        public Visibility GraphAxisLabelsVisibility
        {
            get { return _graphaxislabelsvisibility; }
            set
            {
                if (CheckPropertyChanged<Visibility>("GraphAxisLabelsVisibility", ref _graphaxislabelsvisibility, ref value))
                {
                }
            }
        }

    }
}
