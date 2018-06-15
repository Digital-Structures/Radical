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
        //Legibily reference directions
        public enum Direction {X,Y,Z}
        
        public DesignSurface(IGH_Param param, NurbsSurface surf, double min=-1.0, double max =1.0)
        {
            this.Parameter = param;
            this.Parameter.RemoveAllSources();
            this.Surface = surf;
            this.OriginalSurface = new NurbsSurface(surf);
            BuildVariables(min, max);
        }

        // obsolete or to be made obsolete
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

        public void BuildVariables(double min, double max)
        {
            Variables = new List<IGeoVariable>();
            for (int i = 0; i < Surface.Points.CountU; i++)
            {
                for (int j = 0; j < Surface.Points.CountV; j++)
                {
                    Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.X, this));
                    Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.Y, this));
                    Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.Z, this));
                }
            }

        }

        // obsolete or to be made obsolete
        public void BuildVariables(List<Tuple<int, int>> fptsX, List<Tuple<int, int>> fptsY, List<Tuple<int, int>> fptsZ, double min, double max)
        {
            Variables = new List<IGeoVariable>();
            for (int i=0;i<Surface.Points.CountU;i++)
            {
                for (int j=0;j<Surface.Points.CountV;j++)
                {
                    Tuple<int, int> cp = new Tuple<int, int>(i, j);
                    if (!fptsX.Contains(cp)) { Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.X, this)); }
                    if (!fptsY.Contains(cp)) { Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.Y, this)); }
                    if (!fptsZ.Contains(cp)) { Variables.Add(new SurfaceVariable(min, max, i, j, (int)Direction.Z, this)); }
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
                case (int)Direction.X:
                    newpoint.X = newpoint.X+srfvar.CurrentValue;
                    break;
                case (int)Direction.Y:
                    newpoint.Y = newpoint.Y+srfvar.CurrentValue;
                    break;
                case (int)Direction.Z:
                    newpoint.Z = newpoint.Z+ srfvar.CurrentValue;
                    break;
            }
            this.Surface.Points.SetControlPoint(srfvar.u, srfvar.v, newpoint);
        }
    }
}
