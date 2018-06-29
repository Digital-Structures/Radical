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
    public class DesignCurve : IDesignGeometry
    {
        //Legibily reference directions
        public enum Direction { X, Y, Z }

        public DesignCurve(IGH_Param param, NurbsCurve crv, double min = -1.0, double max = 1.0)
        {
            this.Parameter = param;
            this.Parameter.RemoveAllSources();
            this.Curve = crv;
            this.OriginalCurve = new NurbsCurve(crv);
            BuildVariables(min, max);
        }

        // Obsolete or to be made obsolete
        public DesignCurve(IGH_Param param, List<int> fptsX, List<int> fptsY, double min, double max, NurbsCurve crv)
        {
            this.Parameter = param;
            this.Parameter.RemoveAllSources();
            this.Curve = crv;
            this.OriginalCurve = new NurbsCurve(crv);
            BuildVariables(fptsX, fptsY, min, max);
        }


        public NurbsCurve OriginalCurve;
        public List<Point3d> OriginalPoints;
        public NurbsCurve Curve;
        public List<Point3d> Points;
        //public List<int> PointsIndices;

        public IGH_Param Parameter
        {
            get; set;
        }

        public GH_PersistentGeometryParam<GH_Curve> CrvParameter { get { return Parameter as GH_PersistentGeometryParam<GH_Curve>; } }

        public List<GeoVariable> Variables { get; set; }


        public void BuildVariables(double min, double max)
        {
            Points = Curve.Points.Distinct().Select(x => x.Location).ToList();
            OriginalPoints = OriginalCurve.Points.Distinct().Select(x => x.Location).ToList();
            //PointsIndices = Points.Select(x => Curve.Points.ToList().IndexOf(x)).ToList();
            Variables = new List<GeoVariable>();
            for (int i = 0; i < Points.Count; i++)
            {
                Variables.Add(new CurveVariable(min, max, i, (int)Direction.X, this));
                Variables.Add(new CurveVariable(min, max, i, (int)Direction.Y, this));
                Variables.Add(new CurveVariable(min, max, i, (int)Direction.Z, this));
            }
        }

        // to be made obsolete
        public void BuildVariables(List<int> fptsX, List<int> fptsY, double min, double max)
        {
            Points = Curve.Points.Distinct().Select(x => x.Location).ToList();
            OriginalPoints = OriginalCurve.Points.Distinct().Select(x => x.Location).ToList();
            //PointsIndices = Points.Select(x => Curve.Points.ToList().IndexOf(x)).ToList();
            NurbsCurve crv = new NurbsCurve(Curve.Points.ControlPolygon().ToNurbsCurve());
            Variables = new List<GeoVariable>();
            for (int i = 0; i < Points.Count; i++)
            {
                if (!fptsX.Contains(i)) { Variables.Add(new CurveVariable(min, max, i, (int)Direction.X, this)); }
                if (!fptsX.Contains(i)) { Variables.Add(new CurveVariable(min, max, i, (int)Direction.Y, this)); }
                if (!fptsX.Contains(i)) { Variables.Add(new CurveVariable(min, max, i, (int)Direction.Z, this)); }
            }
        }

        public void Update()
        {
            Curve = NurbsCurve.Create(true, OriginalCurve.Degree, Points);
            CrvParameter.PersistentData.Clear();
            CrvParameter.PersistentData.Append(new Grasshopper.Kernel.Types.GH_Curve(this.Curve));
        }

        public void VarUpdate(GeoVariable geovar)
        {
            CurveVariable crvvar = (CurveVariable)geovar;
            Point3d newpoint = OriginalPoints[crvvar.u];
            int n = Curve.Points.Distinct<ControlPoint>().Count();

            switch (crvvar.Dir)
            {
                case (int)Direction.X:
                    newpoint.X = newpoint.X + crvvar.CurrentValue;
                    newpoint.Y = Points[crvvar.u].Y;
                    newpoint.Z = Points[crvvar.u].Z;
                    break;

                case (int)Direction.Y:
                    newpoint.X = Points[crvvar.u].X;
                    newpoint.Y = newpoint.Y + crvvar.CurrentValue;
                    newpoint.Z = Points[crvvar.u].Z;
                    break;

                case (int)Direction.Z:
                    newpoint.X = Points[crvvar.u].X;
                    newpoint.Y = Points[crvvar.u].Y;
                    newpoint.Z = newpoint.Z + crvvar.CurrentValue;
                    break;
            }

            Points[crvvar.u] = newpoint;
            bool closed = this.Curve.IsClosed;
        }

    }
}
