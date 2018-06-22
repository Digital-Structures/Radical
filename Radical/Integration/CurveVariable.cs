using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;


namespace Radical.Integration
{
    //Represents a single control point degree of freedom on a NURBS Curve
    //(***Curves can probably be abstracted to SurvaceVariable objects where v=0***)
    public class CurveVariable : IGeoVariable
    {
        //dir: The direction in which the point can move
        //min,max: The positional bounds of the control point
        //u, v: NURBS coordinates of the control point
        public int u;
        private int dir;

        public CurveVariable(double min, double max, int u, int dir, DesignCurve crv)
        {
            this.Min = min;
            this.Max = max;
            this.u = u;
            this.dir = dir;
            this.Geometry = crv;
        }

        public int Dir
        {
            get { return this.dir; }
            set { this.dir = value; }
        }

        public double Max { get; set; }
        public double Min { get; set; }

        public double CurrentValue
        {
            get;set;
        }

        public IDesignGeometry Geometry { get; set; }


        // ARCHITECTURE FLAW
        public IGH_Param Parameter
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }


        public bool IsActive
        {
            get; set;
        }

        public void UpdateMin(double x)
        {
            this.Min = x;
        }

        public void UpdateMax(double x)
        {
            this.Max = x;
        }

        public double Gradient()
        {
            throw new NotImplementedException();
        }

        public void UpdateValue(double x)
        {
            this.CurrentValue = x;
            Geometry.VarUpdate(this);
        }
    }
}
