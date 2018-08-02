using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        //CONSTRUCTOR
        public StepperGraphVM(StepperVM VM)
        {
            this.StepperVM = VM;
            InitializeGraphSeries();
        }

        //OBJECTIVE SERIES
        //Collection of objective evolution data series to be displayed
        private SeriesCollection GraphSeries;
        public SeriesCollection ObjectiveSeries
        {
            get { return this.GraphSeries; }
        }

        //INITIALIZE GRAPH SERIES
        public void InitializeGraphSeries()
        {
            //Series Collection for objective graphs
            this.GraphSeries = new SeriesCollection();
            foreach (ChartValues<double> objective in this.StepperVM.ObjectiveEvolution)
            {
                //Make a line series for the given objective
                GraphSeries.Add(new LineSeries
                {
                    Title = "Objective Name",
                    Values = objective,
                    LineSmoothness = 0
                });
            }
        }
    }
}
