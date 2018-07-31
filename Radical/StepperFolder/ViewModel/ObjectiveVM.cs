using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical;

namespace Stepper
{
    //OBJECTIVE VM
    //View Model for storing properties of input objectives
    public class ObjectiveVM : BaseVM
    {
        private StepperVM Stepper;
        public int index;

        //CONSTRUCTOR
        public ObjectiveVM(double value, StepperVM stepper)
        {
            this._name = "Objective";
            this._val = value;
            this.index = 0;
            this.Stepper = stepper;
        }

        //NAME
        //The name of the objective function
        private string _name;
        public string Name
        {
            get { return this._name; }
            set
            {
                //Update the name of the objective and notify property changed
                if (CheckPropertyChanged<string>("Name", ref _name, ref value))
                    this.Stepper.ObjectiveNamesChanged();
            }
        }

        //VALUE
        //The current value of the objective function
        //Read only, can't be changed by the user
        private double _val;
        public double Value
        {
            get { return this._val; }
        }

        //IS ACTIVE
        //Boolean, indicates whether the specified objective is the one to be optimized
        //Only one objective may be active at a time
        private bool _active;
        public bool IsActive
        {
            get { return this._active; }
            set
            {
                //If this objective is activated, deactivate all other objectives
                if (CheckPropertyChanged<bool>("IsActive", ref _active, ref value) && value)
                {
                    foreach (ObjectiveVM obj in this.Stepper.Objectives)
                        if (obj != this)
                            obj.IsActive = false;
                }
            }
        }

    }
}
