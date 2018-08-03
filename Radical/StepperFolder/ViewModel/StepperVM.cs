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
    public class StepperVM : BaseVM
    {
        //Variables and properties
        private DSOptimizerComponent MyComponent;

        public ChartValues<ChartValues<double>> ObjectiveEvolution;
        public List<List<double>> VariableEvolution;
        public List<ObjectiveVM> Objectives;
        public List<VarVM> Variables;

        public GraphVM ObjectiveChart;
        public Design Design;

        public StepperVM() { }

        //CONSTRUCTOR
        public StepperVM(Design design, DSOptimizerComponent stepper)
        {
            //StepperComponent
            this.MyComponent = stepper;

            //DesignSystem
            this.Design = design;
            this.index = 0;
            this.step = 0.05;

            this.trackedstep = 1;

            //Set up Objective View Models and list of objective value evolution 
            this.ObjectiveEvolution = new ChartValues<ChartValues<double>>();
            this.Objectives = new List<ObjectiveVM>();
            int i = 0;

            //Copied what was done to set up num vars in display
            //Still not best solution --> consider creating a seperate list in design 
            foreach (double objective in this.Design.Objectives)
            {
                ObjectiveVM Obj = new ObjectiveVM(objective, this);
                Obj.Name = this.MyComponent.Params.Input[0].Sources[i].Name;
                Obj.IsActive = (this.ObjIndex == i); //Active objective specified by component input parameter

                this.Objectives.Add(Obj);
                this.ObjectiveEvolution.Add(new ChartValues<double> { objective });
                i++;
            }

            //Set up Variable View Models and list of variable value evolution 
            this.VariableEvolution = new List<List<double>>();
            this.Variables = new List<VarVM>();
            foreach (IVariable var in this.Design.Variables)
            {
                this.Variables.Add(new VarVM(var));
                this.VariableEvolution.Add(new List<double> { var.CurrentValue });
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
            set { CheckPropertyChanged<int>("TrackedStep", ref trackedstep, ref value); }
        }

        //NUM STEPS
        //The number of steps taken so far (for graph tracking purposes)
        public int NumSteps
        {
            get { return this.ObjectiveEvolution[0].Count - 1; }
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

            foreach (VarVM var in this.Variables)
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
        }

        //ON WINDOW CLOSING
        //Alert the component that the window has been closed
        //(and therefore a new window can open on double click)
        public void OnWindowClosing()
        {
            this.MyComponent.IsWindowOpen = false;
        }
    }
}
