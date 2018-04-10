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
    public class DesignSurface : IDesignGeometry
    {
        public DesignSurface() { }
        public DesignSurface(IGH_Param param, List<Tuple<int, int>> fptsX, List<Tuple<int, int>> fptsY, List<Tuple<int, int>> fptsZ, double min, double max, NurbsSurface surf)
        {
            this.Parameter = param;
            this.Parameter.RemoveAllSources();
            this.Surface = surf;
            this.OriginalSurface = new NurbsSurface(surf);
            BuildVariables(fptsX, fptsY, fptsZ,min,max);
        }

        public DesignSurface(IGH_Param param)
        {
            this.Parameter = param;
        }

        public NurbsSurface OriginalSurface;

        public NurbsSurface Surface;

        public IGH_Param Parameter
        {
            get;set;
        }
        public GH_PersistentGeometryParam<GH_Surface> SrfParameter { get { return Parameter as GH_PersistentGeometryParam<GH_Surface>; } }

        public List<IGeoVariable> Variables { get; set; }


        public void BuildVariables(List<Tuple<int, int>> fptsX, List<Tuple<int, int>> fptsY, List<Tuple<int, int>> fptsZ, double min, double max)
        {
            Variables = new List<IGeoVariable>();
            for (int i=0;i<Surface.Points.CountU;i++)
            {
                for (int j=0;j<Surface.Points.CountV;j++)
                {
                    Tuple<int, int> cp = new Tuple<int, int>(i, j);
                    if (!fptsX.Contains(cp)) { Variables.Add(new SurfaceVariable(min, max, i, j, 0, this)); }
                    if (!fptsY.Contains(cp)) { Variables.Add(new SurfaceVariable(min, max, i, j, 1, this)); }
                    if (!fptsZ.Contains(cp)) { Variables.Add(new SurfaceVariable(min, max, i, j, 2, this)); }
                }
            }
        }

        public void Update()
        {
            SrfParameter.PersistentData.Clear();
            SrfParameter.PersistentData.Append(new Grasshopper.Kernel.Types.GH_Surface(this.Surface));
        }

        public void VarUpdate(IGeoVariable geovar)
        {
            SurfaceVariable srfvar = (SurfaceVariable)geovar;
            Point3d newpoint = this.OriginalSurface.Points.GetControlPoint(srfvar.u, srfvar.v).Location;
            switch (srfvar.dir)
            {
                case 0:
                    newpoint.X = newpoint.X+srfvar.CurrentValue;
                    break;
                case 1:
                    newpoint.Y = newpoint.Y+srfvar.CurrentValue;
                    break;
                case 2:
                    newpoint.Z = newpoint.Z+ srfvar.CurrentValue;
                    break;
            }
            this.Surface.Points.SetControlPoint(srfvar.u, srfvar.v, newpoint);
        }
    }
}
