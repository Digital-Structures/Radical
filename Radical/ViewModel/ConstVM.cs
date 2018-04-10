using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical.Integration;

namespace Radical
{
    public enum ConstraintType { lessthan, morethan, equalto };

    public class ConstVM:BaseVM
    {
        public ConstVM()
        {
        }

        public ConstVM(IConstraint constraint)
        {
            this.Constraint = constraint;
            this.Name = "C." + Constraint.ConstraintIndex.ToString();
        }

        public IConstraint Constraint;

        private double _currentvalue;
        public double CurrentValue
        {
            get
            { return _currentvalue; }
            set
            {
                if (CheckPropertyChanged<double>("CurrentValue", ref _currentvalue, ref value))
                {
                }
            }

        }

        private double _constraintlimit;
        public double ConstraintLimit
        {
            get
            { return _constraintlimit; }
            set
            {
                if (CheckPropertyChanged<double>("ConstraintLimit", ref _constraintlimit, ref value))
                {
                }
            }

        }

        private string _name;
        public string Name
        {
            get
            { return _name; }
            set
            {
                if (CheckPropertyChanged<string>("Name", ref _name, ref value))
                {
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
                }
            }
        }
        
        private ConstraintType _constraintType;
        public ConstraintType ConstraintType
        {
            get
            { return _constraintType; }
            set
            {
                if (CheckPropertyChanged("ConstraintType", ref _constraintType, ref value))
                {
                    Constraint.ConstraintType = ConstraintType;
                }
            }
        }
    }
}
