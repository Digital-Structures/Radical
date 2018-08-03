using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical.Integration;
using NLoptNet;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;
using Radical;
using Grasshopper;
using Grasshopper.Kernel.Special;

namespace DSOptimization
{
    public class GroupVarVM:VarVM
    {
        //CONSTRUCTOR
        public GroupVarVM(IOptimizeToolVM VM, int dir, int geoIndex=0): base(new SliderVariable(new GH_NumberSlider()))
        {
            this._dir = (Direction)dir;

            //Create a list of the variables affected by the global control
            if(this._dir == Direction.None)
                this.MyVars = VM.NumVars;
            else
                this.MyVars = VM.GeoVars[geoIndex].Where(var => var.Dir == this.Dir).ToList();

            this._value = this.MyVars[0].Value;
            this._min = this.MyVars[0].Min;
            this._max = this.MyVars[0].Max;

        }
        public List<VarVM> MyVars;

        //DIRECTION
        //Direction of the variable group
        private Direction _dir;
        public override int Dir
        {
            get { return (int)this._dir; }
            set { this._dir = (Direction)value; }
        }

        //VALUE
        //Control the value of all grouped variables
        private double _value;
        public override double Value
        {
            get
            { return _value; }
            set
            {
                //Update value if change is in bounds
                if (value <= this.Max && value >= this.Min &&
                    CheckPropertyChanged<double>("Value", ref _value, ref value))
                {
                    foreach (VarVM var in this.MyVars)
                        var.Value = value;

                    //Refresh to change value on the grasshopper canvas
                    Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true, Grasshopper.Kernel.GH_SolutionMode.Silent);
                }
            }

        }

        //MIN
        //Control the minimum of all grouped variables
        private double _min;
        public override double Min
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
                    foreach (VarVM var in this.MyVars)
                        var.Min = value;
                }
            }
        }

        //MAX
        //Control the maximum of all grouped variables
        private double _max;
        public override double Max
        {
            get
            { return _max; }
            set
            {
                //Invalid Bounds, display an error
                if (value < this._min)
                {
                    System.Windows.MessageBox.Show(String.Format("Incompatible bounds!\n" +
                                                                    "Min:{0} > Max:{1}\n", this._min, value));
                }

                else if (CheckPropertyChanged<double>("Max", ref _max, ref value))
                {
                    foreach (VarVM var in this.MyVars)
                        var.Max = value;
                }
            }
        }

        //OPTIMIZATION STARTED
        public void OptimizationStarted()
        {
            this.ChangesEnabled = false;

            foreach (VarVM var in this.MyVars)
                var.ChangesEnabled = false;
        }

        //OPTIMIZATION FINISHED
        public override void OptimizationFinished()
        {
            this.ChangesEnabled = true;

            foreach (VarVM var in this.MyVars)
                var.OptimizationFinished();
        }
    }
}
