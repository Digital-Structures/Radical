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
    public class VarVM : BaseVM
    {
        public VarVM()
        {
        }

        public VarVM(IVariable dvar)
        {
            DesignVar = dvar;
            this.Name = DesignVar.Parameter.NickName;
            this.Value=DesignVar.CurrentValue;
            this.Min = DesignVar.Min;
            this.Max = DesignVar.Max;
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
                    DesignVar.Parameter.NickName = Name;
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
                    DesignVar.UpdateValue(Value);
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
                if (CheckPropertyChanged<double>("Min", ref _min, ref value))
                {
                    DesignVar.UpdateMin(Min);
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
                if (CheckPropertyChanged<double>("Max", ref _max, ref value))
                {
                    DesignVar.UpdateMax(Max);
                }
            }

        }

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
                    DesignVar.IsActive = IsEnabled;
                }
            }
        }
    }
}
