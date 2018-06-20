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

        private string _name;
        public string Name
        {
            get
            { return _name; }
            set
            {
                if (CheckPropertyChanged<string>("Name", ref _name, ref value))
                {
                    DesignVar.Parameter.NickName = this._name;
                }
            }

        }

        private double _value;
        public double Value
        {
            get
            { return _value; }
            set
            {
                if (CheckPropertyChanged<double>("Value", ref _value, ref value))
                {
                    DesignVar.UpdateValue(this._value);
                }
            }

        }

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
                                                                    "Min:{0} > Max:{1}\n" +
                                                                    "Resetting Min to {2}\n", value, this._max, this._min));
                }

                else if (CheckPropertyChanged<double>("Min", ref _min, ref value))
                {
                    DesignVar.UpdateMin(this._min);

                    //Ensure the value of the slider is not outside the new min bound
                    if (this._min > DesignVar.CurrentValue)
                    {
                        DesignVar.UpdateValue(this._min);
                        this.Value = this._min;
                        //NEED SOME KIND OF REFRESH AFTER THIS UPDATE
                    }
                }
            }

        }

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
                        //NEED SOME KIND OF REFRESH AFTER THIS UPDATE
                    }
                }
            }

        }

        //Bool IsEnabled
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
