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
    public class SurfaceVariable : IGeoVariable
    {
        public SurfaceVariable() { }
        public SurfaceVariable(double min, double max, int u, int v, int dir, DesignSurface surf)
        {
            this.Min = min;
            this.Max = max;
            this.u = u;
            this.v = v;
            this.dir = dir;
            this.Geometry = surf;
        }

        public int u;
        public int v;
        public int dir; //0-x;1-y;2-z

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
