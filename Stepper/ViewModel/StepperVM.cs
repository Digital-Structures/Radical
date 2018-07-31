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


namespace Stepper
{
    //STEPPER VM
    //View Model to mediate communication between StepperWindow and StepperComponent
    public class StepperVM : BaseVM
    {
        //Variables and properties
        private StepperComponent MyComponent;
        public ChartValues<ChartValues<double>> ObjectiveEvolution;
        public List<List<double>> VariableEvolution;
        public List<ObjectiveVM> Objectives;
        public List<VarVM> Variables;
        public StepperGraphVM ObjectiveChart;
        private StepperDesign Design;

        public StepperVM() { }

        //CONSTRUCTOR
        public StepperVM(StepperComponent stepper, StepperDesign design)
        {
            //StepperComponent
            this.MyComponent = stepper;

            //DesignSystem
            this.Design = design;

            //Set up Objective View Models and list of objective value evolution 
            this.ObjectiveEvolution = new ChartValues<ChartValues<double>>();
            this.Objectives = new List<ObjectiveVM>();
            int i = 0;
            foreach (double objValue in this.MyComponent.Objectives)
            {
                ObjectiveVM Obj = new ObjectiveVM(objValue, this);
                Obj.IsActive = (this.MyComponent.ObjIndex == i); //Active objective specified by component input parameter
                this.Objectives.Add(Obj);

                this.ObjectiveEvolution.Add(new ChartValues<double> { objValue });
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

        //OPTIMIZE
        public void Optimize()
        {
            Optimizer optimizer = new Optimizer(this.Design);
            optimizer.Optimize();

            //Update value evolution
            int i = 0;
            foreach (ChartValues<double> objective in this.ObjectiveEvolution)
            {
                objective.Add(this.Design.Objectives[i]);
                i++;
            }
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
