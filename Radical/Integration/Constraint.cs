using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical.TestComponents;

namespace Radical.Integration
{
    public class Constraint
    {
        public Constraint()
        {
        }

        public Constraint(RadicalComponent mycomponent, ConstraintType constraintType, int constraintindex, 
            double limit = 0, bool active = true)
        {
            MyComponent = mycomponent;
            this.ConstraintIndex = constraintindex;
            this.ConstraintType = constraintType;
            this.LimitValue = limit;
            this.IsActive = active;
        }

        public RadicalComponent MyComponent { get; set; }
        public double CurrentValue { get { return MyComponent.Constraints[ConstraintIndex]; } }
        public double LimitValue { get; set; }
        public ConstraintType ConstraintType { get; set; }
        public int ConstraintIndex { get; set; }
        public double Gradient { get; set; }
        public bool IsActive;
    }
}
