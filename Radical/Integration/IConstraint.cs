using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical.TestComponents;

namespace Radical.Integration
{
    public interface IConstraint
    {
        double CurrentValue { get; }
        double LimitValue { get; set; }
        RadicalComponent MyComponent { get; set; } 
        ConstraintType ConstraintType { get; set; }
        double Gradient { get; set; }
        int ConstraintIndex { get; set; }
    }

    public class Constraint : IConstraint
    {
        public Constraint()
        {
        }

        public Constraint(RadicalComponent mycomponent, double limit, ConstraintType constraintType, int constraintindex)
        {
            MyComponent = mycomponent;
            this.ConstraintIndex = constraintindex;
            this.ConstraintType = constraintType;
            this.LimitValue = limit;
        }

        public RadicalComponent MyComponent { get; set; }
        public double CurrentValue { get { return MyComponent.Constraints[ConstraintIndex]; }}
        public double LimitValue { get; set; }
        public ConstraintType ConstraintType { get; set; }
        public int ConstraintIndex { get; set; }
        public double Gradient { get; set; }
    }
}
