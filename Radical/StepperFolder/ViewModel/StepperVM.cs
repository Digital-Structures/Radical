using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Media;
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

        public ChartValues<ChartValues<double>> ObjectiveEvolution_Norm;
        public ChartValues<ChartValues<double>> ObjectiveEvolution_Abs;
        public List<List<double>> VariableEvolution;
        public List<List<double>> GradientEvolution;

        public List<ObjectiveVM> Objectives;
        public List<VarVM> Variables { get; set; }
        public List<VarVM> NumVars { get; set; }
        public List<List<VarVM>> GeoVars { get; set; }
        public List<GroupVarVM> GroupVars { get; set; }

        public StepperGraphVM ObjectiveChart_Norm;
        public StepperGraphVM ObjectiveChart_Abs;
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

            //Warn user that system can't handle constraints
            this.opendialog = false;
            if (this.Design.Constraints.Any())
                this.OpenDialog = true;

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
            this.ObjectiveEvolution_Norm = new ChartValues<ChartValues<double>>();
            this.ObjectiveEvolution_Abs = new ChartValues<ChartValues<double>>();
            this.Objectives = new List<ObjectiveVM>();
            int i = 0;

            //Set up list of objective evolution
            foreach (double objective in this.Design.Objectives)
            {
                ObjectiveVM Obj = new ObjectiveVM(objective, this);
                Obj.Name = this.Component.Params.Input[0].Sources[i].NickName;
                Obj.IsActive = (this.ObjIndex == i);

                this.Objectives.Add(Obj);
                this.ObjectiveEvolution_Norm.Add(new ChartValues<double> { 1 });
                this.ObjectiveEvolution_Abs.Add(new ChartValues<double> { objective });
                i++;
            }

            //Set up list of variable value and gradient evolution 
            this.VariableEvolution = new List<List<double>>();
            this.GradientEvolution = new List<List<double>>();
            foreach (VarVM var in this.Variables)
            {
                this.VariableEvolution.Add(new List<double> { var.Value });
                this.GradientEvolution.Add(new List<double> { });
            }

            //Set up both charts
            this.ObjectiveChart_Norm = new StepperGraphVM(ObjectiveEvolution_Norm);
            this.ObjectiveChart_Abs = new StepperGraphVM(ObjectiveEvolution_Abs);
            this.ObjectiveNamesChanged();
        }

        //OPEN DIALOG
        //Boolean to notify user if he's entered constraints
        private bool opendialog;
        public virtual bool OpenDialog
        {
            get { return this.opendialog; }
            set { CheckPropertyChanged<bool>("OpenDialog", ref opendialog, ref value); }
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

        //OBJECTIVE NAME
        public string CurrentObjectiveName
        {
            get { return this.Objectives[ObjIndex].Name; }
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
                {
                    this.ObjectiveChart_Norm.GraphStep = value;
                    this.ObjectiveChart_Abs.GraphStep = value;
                }
                    
            }
        }

        //NUM STEPS
        //The number of steps taken so far (for graph tracking purposes)
        public int NumSteps
        {
            get { return this.ObjectiveEvolution_Norm[0].Count - 1; }
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

            if (this.ObjectiveChart_Abs != null)
            {
                int i = 0;
                foreach (LineSeries objectiveSeries in this.ObjectiveChart_Norm.ObjectiveSeries)
                {
                    objectiveSeries.SetBinding(LineSeries.TitleProperty, new Binding { Source = ObjectiveNames[i] });
                    ((LineSeries)this.ObjectiveChart_Abs.ObjectiveSeries[i]).SetBinding(LineSeries.TitleProperty, new Binding { Source = ObjectiveNames[i] });

                    i++;
                }
            }
        }

        //OPTIMIZE
        public void Optimize(StepperOptimizer.Direction dir, List<List<double>> GradientData)
        {
            StepperOptimizer optimizer = new StepperOptimizer(this.Design, this.ObjIndex, dir, this.StepSize);
            optimizer.Optimize(GradientData);

            //Update variable values at the end of the optimization
            foreach (GroupVarVM var in this.GroupVars)
                var.OptimizationFinished();
            foreach (List<VarVM> geo in this.GeoVars)
                foreach (VarVM var in geo)
                    var.OptimizationFinished();

            //Update objective evolution
            int i = 0;
            foreach (ChartValues<double> objective in this.ObjectiveEvolution_Abs)
            {
                double value = this.Design.Objectives[i];
                objective.Add(value);

                double normalized = value / objective[0];
                this.ObjectiveEvolution_Norm[i].Add(normalized);

                i++;
            }

            //Store corresponding variable values for potential reset
            i = 0;
            foreach (VarVM var in this.Variables)
            {
                this.VariableEvolution[i].Add(var.Value);
                i++;
            }

            //Rescale X-Axis every 10 steps for appearance
            FirePropertyChanged("NumSteps");
            this.ObjectiveChart_Norm.XAxisSteps = this.NumSteps / 10 + 1;
            this.ObjectiveChart_Abs.XAxisSteps = this.NumSteps / 10 + 1;
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
