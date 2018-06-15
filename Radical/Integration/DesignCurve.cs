﻿using System;
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

        public List<IGeoVariable> Variables { get; set; }


        public void BuildVariables(double min, double max)
        {
            Points = Curve.Points.Distinct().Select(x => x.Location).ToList();
            OriginalPoints = OriginalCurve.Points.Distinct().Select(x => x.Location).ToList();
            //PointsIndices = Points.Select(x => Curve.Points.ToList().IndexOf(x)).ToList();
            Variables = new List<IGeoVariable>();
            for (int i = 0; i < Points.Count; i++)
            {
                Variables.Add(new CurveVariable(min, max, i, 0, this));
                Variables.Add(new CurveVariable(min, max, i, 1, this)); 
            }
        }

        // to be made obsolete
        public void BuildVariables(List<int> fptsX, List<int> fptsY, double min, double max)
        {
            Points = Curve.Points.Distinct().Select(x => x.Location).ToList();
            OriginalPoints = OriginalCurve.Points.Distinct().Select(x => x.Location).ToList();
            //PointsIndices = Points.Select(x => Curve.Points.ToList().IndexOf(x)).ToList();
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
            bool closed = this.Curve.IsClosed;
        }

    }
}
