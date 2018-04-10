using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;

namespace Radical.Integration
{
    public interface IVariable
    {
        double CurrentValue { get; set; }
        double Max { get; set; }
        double Min { get; set; }
        void UpdateMin(double x);
        void UpdateMax(double x);
        void UpdateValue(double x);
        double Gradient();
        IGH_Param Parameter { get; set; }       
    }

    public interface IGeoVariable:IVariable
    {
        IDesignGeometry Geometry { get; set; } //geometry to which the variable belongs
    }
}
