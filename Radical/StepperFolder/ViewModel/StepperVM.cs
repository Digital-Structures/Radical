using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using LiveCharts;
using LiveCharts.Wpf;
using System.Threading;
using Radical;
using Radical.Integration;
using DSOptimization;

namespace Stepper
{
    //STEPPER VM
    //View Model to mediate communication between StepperWindow and StepperComponent
    public class StepperVM : BaseVM, IOptimizeToolVM
    {
        //Variables and properties
        public DSOptimizerComponent Component { get; set; }

        public ChartValues<ChartValues<double>> ObjectiveEvolution;
        public List<List<double>> VariableEvolution;

        public List<ObjectiveVM> Objectives;
        public List<VarVM> Variables { get; set; }
        public List<VarVM> NumVars { get; set; }
        public List<List<VarVM>> GeoVars { get; set; }
        public List<GroupVarVM> GroupVars { get; set; }

        public StepperGraphVM ObjectiveChart;
        public Design Design;

        public StepperVM() { }

        //CONSTRUCTOR
        public StepperVM(Design design, DSOptimizerComponent stepper)
        {
            //StepperComponent
            this.Component = stepper;

            //DesignSystem
            this.Design = design;
            this.index = 0;
            this.step = 0.05;
            this.trackedstep = 0;

            //Variable Lists
            //Separate for display
            this.NumVars = new List<VarVM>();
            this.GeoVars = new List<List<VarVM>>();
            this.GroupVars = new List<GroupVarVM>();
            SortVariables();

            //Variable lists
            //Combined for easy value updates
            this.Variables = new List<VarVM>();
            this.Variables.AddRange(this.NumVars);
            this.Variables.AddRange(this.GeoVars.SelectMany(x => x).ToList());
            

            //Set up Objective View Models and list of objective value evolution 
            this.ObjectiveEvolution = new ChartValues<ChartValues<double>>();
            this.Objectives = new List<ObjectiveVM>();
            int i = 0;

            //Copied what was done to set up num vars in display
            //Still not best solution --> consider creating a seperate list in design 
            foreach (double objective in this.Design.Objectives)
            {
                ObjectiveVM Obj = new ObjectiveVM(objective, this);
                Obj.Name = this.Component.Params.Input[0].Sources[i].Name;
                Obj.IsActive = (this.ObjIndex == i); //Active objective specified by component input parameter

                this.Objectives.Add(Obj);
                this.ObjectiveEvolution.Add(new ChartValues<double> { objective });
                i++;
            }

            //Set up list of variable value evolution 
            this.VariableEvolution = new List<List<double>>();
            foreach (VarVM var in this.Variables)
            {
                this.VariableEvolution.Add(new List<double> { var.Value });
            }
        }

        //OBJECTIVE NAMES
        //For combo box drop down
        public List<string> ObjectiveNames
        {
            get
            {
                var names = new List<string>();

                foreach (ObjectiveVM objective in this.Objectives)
                    names.Add(objective.Name);

                return names;
            }
        }

        //OBJECTIVE INDEX
        private int index;
        public int ObjIndex
        {
            get { return index; }
            set { CheckPropertyChanged<int>("ObjIndex", ref index, ref value); }
        }

        //STEP SIZE
        private double step;
        public double StepSize
        {
            get { return step; }
            set { CheckPropertyChanged<double>("StepSize", ref step, ref value); }
        }

        //TRACKED STEP
        //Step number user is tracking with the UI slider
        //To potentially be reverted back to
        private int trackedstep;
        public int TrackedStep
        {
            get { return this.trackedstep; }
            set
            {
                if (CheckPropertyChanged<int>("TrackedStep", ref trackedstep, ref value))
                    this.ObjectiveChart.GraphStep = value;
            }
        }

        //NUM STEPS
        //The number of steps taken so far (for graph tracking purposes)
        public int NumSteps
        {
            get { return this.ObjectiveEvolution[0].Count - 1; }
        }

        //SORT VARIABLES
        //Separate geometric and numeric variables
        //Sorting helps with UI stack panel organization
        private void SortVariables()
        {
            //GEOMETRIES
            int geoIndex = 1;
            foreach (IDesignGeometry geo in this.Design.Geometries)
            {
                List<VarVM> singleGeoVars = new List<VarVM> { };

                //Add all the variables for that geometry to a sublist of varVMs
                int varIndex = 0;
                foreach (GeoVariable var in geo.Variables)
                {
                    VarVM geoVar = new VarVM(var);
                    int dir = var.Dir;

                    //Logical default naming of variable
                    //e.g. G1.u1v1.X
                    geoVar.Name += ((GeoVariable)geoVar.DesignVar).PointName;

                    singleGeoVars.Add(geoVar);
                    varIndex++;
                }

                this.GeoVars.Add(singleGeoVars);
                geoIndex++;
            }

            //SLIDERS
            /***This is probably not the best way to do this as it involves looping over geometry variables already stored***/
            foreach (var numVar in this.Design.Variables.Where(numVar => numVar is SliderVariable))
            {
                this.NumVars.Add(new VarVM(numVar));
            }
        }

        //Objective Names Changed
        public void ObjectiveNamesChanged()
        {
            FirePropertyChanged("ObjectiveNames");           
        }

        //OPTIMIZE
        public void Optimize(StepperOptimizer.Direction dir)
        {
            StepperOptimizer optimizer = new StepperOptimizer(this.Design, this.ObjIndex, dir, this.StepSize);
            optimizer.Optimize();

            //Update variable values at the end of the optimization
            foreach (GroupVarVM var in this.GroupVars)
                var.OptimizationFinished();
            foreach (List<VarVM> geo in this.GeoVars)
                foreach (VarVM var in geo)
                    var.OptimizationFinished();

            //Update objective evolution
            int i = 0;
            foreach (ChartValues<double> objective in this.ObjectiveEvolution)
            {
                objective.Add(this.Design.Objectives[i]);
                i++;
            }

            //Store corresponding variable values for potential reset
            i = 0;
            foreach (VarVM var in this.Variables)
            {
                this.VariableEvolution[i].Add(var.Value);
                i++;
            }

            FirePropertyChanged("NumSteps");
            this.ObjectiveChart.XAxisSteps = this.NumSteps / 10 + 1;
        }

        //RESET
        //Allow user to return to previous step systems
        public void Reset()
        {
            var step = this.TrackedStep;

            int i = 0;
            foreach(VarVM var in this.Variables)
            {
                var.Value = this.VariableEvolution[i][step];
                i++;
            }
        }

        //ON WINDOW CLOSING
        //Alert the component that the window has been closed
        //(and therefore a new window can open on double click)
        public void OnWindowClosing()
        {
            this.Component.IsWindowOpen = false;
        }
    }
}
