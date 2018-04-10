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
        public DesignCurve() { }
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
        public List<int> PointsIndices;

        public IGH_Param Parameter
        {
            get; set;
        }
        public GH_PersistentGeometryParam<GH_Curve> CrvParameter { get { return Parameter as GH_PersistentGeometryParam<GH_Curve>; } }

        public List<IGeoVariable> Variables { get; set; }


        public void BuildVariables(List<int> fptsX, List<int> fptsY, double min, double max)
        {
            Points = Curve.Points.Distinct().Select(x=>x.Location).ToList();
            OriginalPoints = OriginalCurve.Points.Distinct().Select(x => x.Location).ToList();
            PointsIndices = Points.Select(x => Curve.Points.ToList().IndexOf(x)).ToList();
            NurbsCurve crv = new NurbsCurve(Curve.Points.ControlPolygon().ToNurbsCurve());
            Variables = new List<IGeoVariable>();
            for (int i = 0; i < Points.Count; i++)
            {
                if (!fptsX.Contains(i)) { Variables.Add(new CurveVariable(min, max, i, 0, this)); }
                if (!fptsY.Contains(i)) { Variables.Add(new CurveVariable(min, max, i, 1, this)); }
            }
        }

        public void Update()
        {
            Curve = NurbsCurve.Create(true, OriginalCurve.Degree, Points);
            CrvParameter.PersistentData.Clear();
            CrvParameter.PersistentData.Append(new Grasshopper.Kernel.Types.GH_Curve(this.Curve));
        }

        public void VarUpdate(IGeoVariable geovar)
        {
            CurveVariable crvvar = (CurveVariable)geovar;
            Point3d newpoint = OriginalPoints[crvvar.u];
            int n = Curve.Points.Distinct<ControlPoint>().Count();

            switch (crvvar.dir)
            {
                case 0:
                    newpoint.X = newpoint.X + crvvar.CurrentValue;
                    newpoint.Y = Points[crvvar.u].Y;

                    break;
                case 1:
                    newpoint.Y = newpoint.Y + crvvar.CurrentValue;
                    newpoint.X = Points[crvvar.u].X;
                    break;
            }

            Points[crvvar.u] = newpoint;
            //if (crvvar.u==0)
            //{
            //    switch (crvvar.dir)
            //    {
            //        case 0:
            //            this.Curve.Translate(crvvar.CurrentValue, 0, 0);
            //            for (int i=0; i<n;i++)
            //            {
            //                this.Curve.Points.SetPoint(i, new Point3d(this.Curve.Points[i].Location-new Point3d(2*crvvar.CurrentValue, 0, 0)));
            //            }
            //            break;
            //        case 1:
            //            this.Curve.Translate( 0, crvvar.CurrentValue, 0);
            //            for (int i = 0; i < n; i++)
            //            {
            //                this.Curve.Points.SetPoint(i, new Point3d(this.Curve.Points[i].Location - new Point3d(0,2 * crvvar.CurrentValue, 0)));
            //            }
            //            break;
            //    }
            //}
            //else
            //{
            //    switch (crvvar.dir)
            //    {
            //        case 0:
            //            newpoint.X = newpoint.X + crvvar.CurrentValue;
            //            break;
            //        case 1:
            //            newpoint.Y = newpoint.Y + crvvar.CurrentValue;
            //            break;
            //    }

            //    this.Curve.Points.SetPoint(crvvar.u, newpoint);

            //}



            //if (crvvar.u == 0) { this.Curve.SetEndPoint(newpoint);}

            bool closed = this.Curve.IsClosed;

        }


    }
}
