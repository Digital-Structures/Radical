using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.GUI;
using Radical.Integration;

namespace Radical
{
    //VARIABLE VIEW MODEL
    //Manages the bounds and values of input variables to be optimized
    public class VarVM : BaseVM
    {
        //CONSTRUCTOR
        //default values obtained from original Grasshopper component variables
        public VarVM(IVariable dvar)
        {
            DesignVar = dvar;
            this._name = DesignVar.Parameter.NickName;
            this._value = DesignVar.CurrentValue;
            this._min = DesignVar.Min;
            this._max = DesignVar.Max;
            this.IsEnabled = true;
        }

        public IVariable DesignVar;

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
                    if (!(DesignVar is IGeoVariable))
                    {
                        DesignVar.Parameter.NickName = this._name;
                    }
                }
            }

        }

        //VALUE
        //Current value of the individual variable
        private double _value;
        public double Value
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
        public double Min
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
                    DesignVar.UpdateMin(this._min);

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
        public double Max
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
                    DesignVar.UpdateMax(_max);

                    //Ensure the value of the slider is not outside the new max bound
                    if (_max < DesignVar.CurrentValue)
                    {
                        DesignVar.UpdateValue(_max);
                        this.Value = _max;
                    }
                }
            }

        }

        //IS ENABLED
        //determines whether variable will be considered in optimization
        private bool _isenabled;
        public bool IsEnabled
        {
            get
            {
                return _isenabled;
            }
            set
            {
                if (CheckPropertyChanged<bool>("IsEnabled", ref _isenabled, ref value))
                {
                    DesignVar.IsActive = this._isenabled;
                }
            }
        }
    }
}
