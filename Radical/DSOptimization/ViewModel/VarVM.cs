﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI;

namespace DSOptimization
{ 
    //VARIABLE VIEW MODEL
    //Manages the bounds and values of input variables to be optimized
    public class VarVM : BaseVM
    {
        public enum Direction { X, Y, Z, None };
        public IVariable DesignVar;

        //Original value of the variable before optimization
        public double OriginalValue { get; set; }
        //Minimum value obtained through the optimization process
        public double BestSolutionValue { get; set; }

        //CONSTRUCTOR
        //default values obtained from original Grasshopper component variables
        public VarVM(IVariable dvar)
        {
            DesignVar = dvar;

            //value trackers
            this.OriginalValue = DesignVar.CurrentValue;
            this.BestSolutionValue = DesignVar.CurrentValue;

            this._name = DesignVar.Parameter.Name;

            this._value = DesignVar.CurrentValue;
            this._min = DesignVar.Min;
            this._max = DesignVar.Max;
            this.IsActive = true;

            this.OptRunning = false;
        }

        //DIRECTION
        private Direction _dir;
        public virtual int Dir
        {
            get
            {
                return this.DesignVar.Dir;
            }
            set 
            {
                this._dir = (Direction)value;
            }
        }

        //NAME
        //The name of an individual variable
        private string _name;
        public string Name
        {
            get
            { return _name; }
            set
            {
                if (CheckPropertyChanged<string>("Name", ref _name, ref value))
                {
                    //Prevent naming geometries after individual control points 
                    if (!(DesignVar is GeoVariable))
                    {
                        DesignVar.Parameter.NickName = this._name;
                    }
                }
            }

        }

        //VALUE
        //Current value of the individual variable
        private double _value;
        public virtual double Value
        {
            get
            { return _value; }
            set
            {
                //Update value if change is in bounds
                if (value<=this.Max && value>=this.Min &&
                    CheckPropertyChanged<double>("Value", ref _value, ref value))
                {
                    DesignVar.UpdateValue(this._value);

                    //Refresh to change value on the grasshopper canvas
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true, Grasshopper.Kernel.GH_SolutionMode.Silent);
                }
            }
        }

        //MIN
        //Minimum value the variable should hold
        private double _min;
        public virtual double Min
        {
            get
            { return _min; }
            set
            {
                //Invalid Bounds, display an error
                if (value > this._max)
                {
                    System.Windows.MessageBox.Show(String.Format("Incompatible bounds!\n" +
                                                                    "Min:{0} > Max:{1}\n", value, this._max));
                }

                else if (CheckPropertyChanged<double>("Min", ref _min, ref value))
                {
                    DesignVar.Min = this._min;

                    //Ensure the value of the slider is not outside the new min bound
                    if (this._min > DesignVar.CurrentValue)
                    {
                        DesignVar.UpdateValue(this._min);
                        this.Value = this._min;
                    }
                }
            }

        }

        //MAX
        //Maximum value the variable should hold
        private double _max;
        public virtual double Max
        {
            get
            { return _max; }
            set
            {
                //Invalid Bounds, display an error
                if (value < this._min)
                {
                    System.Windows.MessageBox.Show(String.Format("Incompatible bounds!\n" +
                                                                    "Max:{0} < Min:{1}\n" +
                                                                    "Resetting Max to {2}\n", value, this._min, this._max));
                }
                else if (CheckPropertyChanged<double>("Max", ref _max, ref value))
                {
                    DesignVar.Max = this._max;

                    //Ensure the value of the slider is not outside the new max bound
                    if (_max < DesignVar.CurrentValue)
                    {
                        DesignVar.UpdateValue(this._max);
                        this.Value = this._max;
                    }
                }
            }
        }

        //IS ACTIVE
        //Determines whether variable will be considered in optimization
        private bool _isactive;
        public virtual bool IsActive
        {
            get
            {
                return _isactive;
            }
            set
            {
                if (CheckPropertyChanged<bool>("IsActive", ref _isactive, ref value))
                {
                    DesignVar.IsActive = this._isactive;
                }
            }
        }

        //GRADIENT
        //Stores the gradient of the variable for a given objective
        private double _grad;
        public double Gradient
        {
            get { return _grad; }
            set { CheckPropertyChanged<double>("Gradient", ref _grad, ref value); }
        }

        //OPTIMIZATION FINISHED
        //Update UI sliders to reflect optimized values
        public virtual void OptimizationFinished()
        {
            this.ChangesEnabled = true;
            this.Value = DesignVar.CurrentValue;
        }

        //UPDATE BEST SOLUTION VALUE 
        //Should be called when the current value of the variable corresponds to the current
        //best solution of the objective
        public void UpdateBestSolutionValue()
        {
            this.BestSolutionValue = this.DesignVar.CurrentValue;
        }

        public void SetBestSolution()
        {
            this.Value = this.BestSolutionValue;
        }

        public void ResetValue()
        {
            this.Value = this.OriginalValue;
            this.BestSolutionValue = this.OriginalValue;
        }
    }
}