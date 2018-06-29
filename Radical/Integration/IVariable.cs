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
        void UpdateValue(double x);
        double Gradient();
        bool IsActive { get; set; }
        IGH_Param Parameter { get; }       
    }

    //public interface IGeoVariable:IVariable
    //{
    //    IDesignGeometry Geometry { get; set; } //geometry to which the variable belongs
    //    int Dir { get; set; }
    //}
}
