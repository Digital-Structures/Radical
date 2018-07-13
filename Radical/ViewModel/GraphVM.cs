using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class GraphVM : BaseVM
    {
        public GraphVM(List<double> scores)
        {
            _graphscores = scores; 
            _linegraph_name = "temp";
            _chartlinex = 0;
            _mouseobjectivevaluedisplay = 1;
            _mouseobjectivevaluedisplayy = 0;
        }

        private List<double> _graphscores;
        public List<double> GraphScores
        {
            get
            { return _graphscores; }
            set
            {
                _graphscores = value;
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
            get
            {
                return _chartlinex;
            }
            set
            {
                if (CheckPropertyChanged<double>("ChartLineX", ref _chartlinex, ref value))
                {
                }
            }
        }

        private double _mouseobjectivevaluedisplay;
        public double MouseObjectiveValueDisplay
        {
            get
            {
                return _mouseobjectivevaluedisplay;
            }
            set
            {
                if (CheckPropertyChanged<double>("MouseObjectiveValueDisplay", ref _mouseobjectivevaluedisplay, ref value))
                {
                    //CalculateActualXPosition();
                }
            }
        }

        private InteractiveDataDisplay.WPF.LineGraph _plotter;
        public InteractiveDataDisplay.WPF.LineGraph Plotter
        {
            get { return _plotter; }
            set
            {
                _plotter = value;
            }
        }

        private Line _chartline;
        public Line ChartLine
        {
            get { return _chartline; }
            set { _chartline = value; }
        }

        //public double 

        private double _mouseobjectivevaluedisplayy;
        public double MouseObjectiveValueDisplayY
        {
            get
            { return _mouseobjectivevaluedisplayy; }
            set
            {
                if (CheckPropertyChanged<double>("MouseObjectiveValueDisplayY", ref _mouseobjectivevaluedisplayy, ref value))
                {
                }
            }
        }

        private bool _showline;
        public bool ShowLine
        {
            get { return _showline; }
            set { _showline = value; }
        }

        public void UpdateLine(int iteration)
        {
            this.MouseObjectiveValueDisplay = iteration;

            if (GraphScores.Any() && ShowLine)
            {
                ChartLineVisibility = Visibility.Visible;
                double yValue = GraphScores.ElementAt(iteration);
                this.MouseObjectiveValueDisplayY = yValue;

                double ScaleX = Plotter.ScaleX;

                //actualX * ScaleX scales the graph value to appropriate mouse position
                //+35 is a hardcoded value because the position is off due to the side of the graph
                //Plotter.OffsetX takes into account if the graph has been moved 
                double newXPosition = iteration * ScaleX + 45 - Plotter.OffsetX;
                //while (newXPosition - 45 < 0)
                //{
                //    iteration++;
                //    newXPosition = iteration * ScaleX + 45 - Plotter.OffsetX;
                //}
                if (newXPosition - 45 < 0 || newXPosition > Plotter.Width)
                {
                    ChartLineVisibility = Visibility.Collapsed;
                }
                this.ChartLineX = newXPosition;
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

    }
}
