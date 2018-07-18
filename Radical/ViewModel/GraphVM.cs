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
        public GraphVM(List<double> scores, string name)
        {
            _graphscores = scores; 
            _linegraph_name = name;
            _chartlinex = 0;
            _x = "0";
            _y = "0";
            _graphVisibility = Visibility.Visible;
            DisplayX = null;
            DisplayY = null;
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
            get { return String.Format("Value: {0}", _y); }
            set
            {
                CheckPropertyChanged<string>("DisplayY", ref _y, ref value);
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

        private bool _showline;
        public bool ShowLine
        {
            get { return _showline; }
            set { _showline = value; }
        }

        public void UpdateLine(int iteration)
        {
            this.DisplayX = iteration.ToString();

            if (GraphScores.Any() && ShowLine)
            {
                ChartLineVisibility(Visibility.Visible);
                double yValue = GraphScores.ElementAt(iteration);
                this.DisplayY = String.Format("{0:0.00}",yValue);

                //Calculations for the horizontal line
                //actualX * ScaleX scales the graph value to appropriate mouse position
                //+45 is a hardcoded value because the position is off due to the side of the graph
                //Plotter.OffsetX takes into account if the graph has been moved 
                double newXPosition = iteration * Plotter.ScaleX + 45 - Plotter.OffsetX;
                
                if (newXPosition - 45 < 0 || newXPosition > Plotter.Width)
                {
                    ChartLineVisibilityX = Visibility.Collapsed;
                }
                this.ChartLineX = newXPosition;

                //Calculatoins for the vertical line
                double newYPosition = -1 * (yValue * Plotter.ScaleY - Plotter.OffsetY);
                if (newYPosition < 0 || newYPosition > Plotter.ActualHeight)
                {
                    ChartLineVisibilityY = Visibility.Collapsed;
                }
                this.ChartLineY = newYPosition;
            }
        }

        //GRAPH VISIBILITY
        //Disables graph visibility during
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
            
            if (ChartLineX - 45 < 0 || ChartLineX > this.Plotter.ActualWidth + 45)
            {
                ChartLineVisibilityX = Visibility.Collapsed;
                DisplayX = null;
                DisplayY = null;
            }
            if (ChartLineY < 0 || ChartLineY > this.Plotter.ActualHeight)
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
            this.ChartLineWidth = this.Plotter.ActualWidth + 45;
        }

    }
}
