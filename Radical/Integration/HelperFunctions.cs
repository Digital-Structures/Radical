using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Radical.TestComponents;

namespace Radical.Integration
{
    public static class HelperFunctions
    {
        public static double tolerance = 1e-3;

        public static Design GenerateDesign(ExploreComponent component)
        {
            List<IVariable> vars = new List<IVariable>();
            foreach (IGH_Param param in component.Params.Input[0].Sources)
            {
                vars.Add(new SliderVariable(param));
            }
            return new Design(vars, component);
        }

        public static Design GenerateDesign(GuideTestOptComponent component)
        {
            List<IVariable> vars = new List<IVariable>();
            foreach (IGH_Param param in component.Params.Input[0].Sources)
            {
                vars.Add(new SliderVariable(param));
            }

            return new Design(vars, component);
        }

        public static Design GenerateDesign(GuideTestSurfOptComponent component)
        {
            List<IVariable> vars = new List<IVariable>();
            List<IDesignGeometry> geos = new List<IDesignGeometry>();
            foreach (IGH_Param param in component.Params.Input[0].Sources)
            {
                List<Tuple<int, int>> fpx = FixedPoints(component.FPtsX, component.Surface);
                List<Tuple<int, int>> fpy = FixedPoints(component.FPtsY, component.Surface);
                List<Tuple<int, int>> fpz = FixedPoints(component.FPtsZ, component.Surface);
                geos.Add(new DesignSurface(param, fpx, fpy, fpz, component.Min, component.Max, component.Surface));
            }

            return new Design(vars, geos, component);
        }

        public static Design GenerateDesign(GuideTestSurfConstOptComponent component)
        {
            List<IVariable> vars = new List<IVariable>();
            List<IDesignGeometry> geos = new List<IDesignGeometry>();
            int i = 0;
            foreach (IGH_Param param in component.Params.Input[0].Sources)
            {
                NurbsSurface surf = component.Surfaces[i];
                List<Tuple<int, int>> fpx = FixedPoints(component.FPtsX, surf);
                List<Tuple<int, int>> fpy = FixedPoints(component.FPtsY, surf);
                List<Tuple<int, int>> fpz = FixedPoints(component.FPtsZ, surf);
                geos.Add(new DesignSurface(param, fpx, fpy, fpz, component.Min[i], component.Max[i], surf));
                i++;
            }

            return new Design(vars, geos, component);
        }

        public static Design GenerateDesign(GuideTestCurveConstOptComponent component)
        {
            List<IVariable> vars = new List<IVariable>();
            List<IDesignGeometry> geos = new List<IDesignGeometry>();
            int i = 0;
            foreach (IGH_Param param in component.Params.Input[0].Sources)
            {
                NurbsCurve crv = component.Curves[i];
                List<int> fpx = FixedPoints(component.FPtsX, crv);
                List<int> fpy = FixedPoints(component.FPtsY, crv);
                geos.Add(new DesignCurve(param, fpx, fpy, component.Min[i], component.Max[i], crv));
                i++;
            }
            return new Design(vars, geos, component);
        }

        public static Design GenerateDesign(RadicalComponent component)
        {
            List<IVariable> vars = new List<IVariable>();
            List<IDesignGeometry> geos = new List<IDesignGeometry>();
            List<IConstraint> consts = new List<IConstraint>();


            // Add all variables
            foreach (IGH_Param param in component.Params.Input[2].Sources)
            {
                vars.Add(new SliderVariable(param));
            }
            for (int i = 0; i < component.Params.Input[4].Sources.Count; i++)
            {
                IGH_Param param = component.Params.Input[4].Sources[i];
                NurbsCurve surf = component.CrvVariables[i];
                geos.Add(new DesignCurve(param, surf));
            }
            for (int i = 0; i < component.Params.Input[3].Sources.Count; i++)
            {
                IGH_Param param = component.Params.Input[3].Sources[i];
                NurbsSurface surf = component.SrfVariables[i];
                geos.Add(new DesignSurface(param, surf));
            }

            // Add Constraints
            for (int i = 0; i < component.Constraints.Count; i++)
            {
                consts.Add(new Constraint(component, 0, ConstraintType.lessthan, i));
            }



            return new Design(vars, geos, consts, component);
        }

        public static List<Tuple<int, int>> FixedPoints(List<Point3d> points, NurbsSurface srf)
        {
            //dumb way to do it but does not really matter for computation time anyway
            if (!points.Any()) { return new List<Tuple<int, int>>(); }

            List<Tuple<int, int>> fpoints = new List<Tuple<int, int>>();
            for (int i = 0; i < srf.Points.CountU; i++)
            {
                for (int j = 0; j < srf.Points.CountV; j++)
                {
                    Point3d pt = srf.Points.GetControlPoint(i, j).Location;
                    List<double> distance = points.Select(x => x.DistanceTo(pt)).ToList();
                    double mindist = distance.Min();
                    if (mindist < tolerance) { fpoints.Add(new Tuple<int, int>(i, j)); }
                }
            }
            return fpoints;
        }

        public static List<int> FixedPoints(List<Point3d> points, NurbsCurve crv)
        {
            if (!points.Any()) { return new List<int>(); }

            List<int> fpoints = new List<int>();
            for (int i = 0; i < crv.Points.Count; i++)
            {
                Point3d pt = crv.Points[i].Location;
                List<double> distance = points.Select(x => x.DistanceTo(pt)).ToList();
                double mindist = distance.Min();
                if (mindist < tolerance) { fpoints.Add(i); }
            }
            return fpoints;
        }


    }
}
