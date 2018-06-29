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
    //SURFACE VARIABLE
    //Represents a single control point degree of freedom on a NURBS Surface
    public class SurfaceVariable : GeoVariable
    {
        //CONSTRUCTOR
        public SurfaceVariable(double min, double max, int u, int v, int dir, DesignSurface surf):base(min,max,dir,surf)
        {
            this.u = u;
            this.v = v;
        }
        public int u;
        public int v;
    }
}
