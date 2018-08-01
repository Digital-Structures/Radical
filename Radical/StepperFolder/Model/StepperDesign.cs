using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical.Integration;
using Radical;
using Grasshopper;
using Grasshopper.Kernel;
using DSOptimization;

namespace Stepper
{
    public class StepperDesign
    {
        public StepperComponent MyComponent;
        public List<IVariable> Variables;

        public StepperDesign(StepperComponent stepper)
        {
            this.MyComponent = stepper;

            this.Variables = new List<IVariable>();
            foreach (IGH_Param param in MyComponent.Params.Input[0].Sources)
                this.Variables.Add(new SliderVariable(param));
        }

        public List<double> Objectives
        {
            get { return MyComponent.Objectives; }
        }
    }
}
