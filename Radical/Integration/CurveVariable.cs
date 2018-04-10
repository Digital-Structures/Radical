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
    public class CurveVariable : IGeoVariable
    {
        public CurveVariable() { }
        public CurveVariable(double min, double max, int u, int dir, DesignCurve crv)
        {
            this.Min = min;
            this.Max = max;
            this.u = u;
            this.dir = dir;
            this.Geometry = crv;
        }

        public int u;
        public int dir; //0-x;1-y

        public double CurrentValue
        {
            get;set;
        }

        public IDesignGeometry Geometry { get; set; }

        public double Max
        {
            get;
            set;
        }

        public double Min
        {
            get;
            set;
        }

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
