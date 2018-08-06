using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Radical;
using DSOptimization;

namespace Stepper
{
    //GRAPH VM
    public class StepperGraphVM : BaseVM
    {
        private StepperVM StepperVM;
        public List<SolidColorBrush> Colors;

        //CONSTRUCTOR
        public StepperGraphVM(StepperVM VM, List<SolidColorBrush> colors)
        {
            this.StepperVM = VM;
            this.Colors = colors;
            this.axissteps = 1;

            InitializeGraphSeries();
        }

        //OBJECTIVE SERIES
        //Collection of objective evolution data series to be displayed
        private SeriesCollection GraphSeries;
        public SeriesCollection ObjectiveSeries
        {
            get { return this.GraphSeries; }
        }

        //TRACKED STEP
        private int step;
        public int GraphStep
        {
            get { return this.step; }
            set
            {
                CheckPropertyChanged<int>("GraphStep", ref step, ref value);
            }
        }

        //XAxisSteps
        private int axissteps;
        public int XAxisSteps
        {
            get { return axissteps; }
            set { CheckPropertyChanged<int>("XAxisSteps", ref axissteps, ref value); }
        }

        //INITIALIZE GRAPH SERIES
        public void InitializeGraphSeries()
        {
            //Series Collection for objective graphs
            this.GraphSeries = new SeriesCollection();

            int i= 0;
            foreach (ChartValues<double> objective in this.StepperVM.ObjectiveEvolution)
            {
                //Make a line series for the given objective
                GraphSeries.Add(new LineSeries
                {
                    Title = this.StepperVM.Objectives[i].Name,
                    Values = objective,
                    Stroke = this.Colors[i],
                    StrokeThickness = 5,
                    Fill = Brushes.Transparent,
                    LineSmoothness = 0,
                    PointGeometrySize = 15
                });

                i++;
                if (i == this.Colors.Count)
                    i = 0;
            }
        }
    }
}
