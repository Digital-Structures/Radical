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
        private List<SolidColorBrush> Colors;

        //CONSTRUCTOR
        public StepperGraphVM(StepperVM VM)
        {
            this.StepperVM = VM;
            this.Colors = new List<SolidColorBrush>();

            InitializeColors();
            InitializeGraphSeries();
        }

        //OBJECTIVE SERIES
        //Collection of objective evolution data series to be displayed
        private SeriesCollection GraphSeries;
        public SeriesCollection ObjectiveSeries
        {
            get { return this.GraphSeries; }
        }

        //INITIALIZE COLORS
        public void InitializeColors()
        {
            var stroke1 = (SolidColorBrush)(new BrushConverter().ConvertFrom("#ff9100"));
            this.Colors.Add(stroke1);

            var stroke2 = (SolidColorBrush)(new BrushConverter().ConvertFrom("#607d8b"));
            this.Colors.Add(stroke2);
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
                    PointGeometrySize = 20
                });

                i++;
                if (i == this.Colors.Count)
                    i = 0;
            }
        }
    }
}
