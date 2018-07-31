using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Radical.Integration;
using Radical.Components;

namespace Radical.TestComponents
{
    public class GuideTestSurfConstOptComponent : GH_Component, IOptimizationComponent
    {

        public double Objective { get; set; }
        public List<double> Constraints { get; set; }
        public List<double> Evolution { get; set; }

        public List<NurbsSurface> Surfaces;
        public List<double> Min = new List<double>();
        public List<double> Max = new List<double>();
        public List<Point3d> FPtsX = new List<Point3d>();
        public List<Point3d> FPtsY = new List<Point3d>();
        public List<Point3d> FPtsZ = new List<Point3d>();


        /// <summary>
        /// Initializes a new instance of the GuideTestSurfOptComponent class.
        /// </summary>
        public GuideTestSurfConstOptComponent()
          : base("GuideTestSurfConstOptComponent", "Guide",
              "Guide",
              "Test", "Guide")
        {
            Objective = 0;
            Evolution = new List<double>();
            Constraints = new List<double>();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Variable Surface", "VarSrf", "Design Surface", GH_ParamAccess.list);
            pManager.AddNumberParameter("Objective", "O", "Objective to Minimize", GH_ParamAccess.item);
            pManager.AddNumberParameter("Upper Bound", "uB", "Upper Bound for Design Variables", GH_ParamAccess.list);
            pManager.AddNumberParameter("Lower Bound", "lB", "Lower Bound for Design Variables", GH_ParamAccess.list);
            pManager.AddPointParameter("Fixed X Points", "pX", "Points with X Position Fixed during Optimization", GH_ParamAccess.list);
            pManager.AddPointParameter("Fixed Y Points", "pY", "Points with Y Position Fixed during Optimization", GH_ParamAccess.list);
            pManager.AddPointParameter("Fixed Z Points", "pZ", "Points with Z Position Fixed during Optimization", GH_ParamAccess.list);
            pManager.AddNumberParameter("Constraints", "Const", "Less or Equal Zero Constraints", GH_ParamAccess.list);
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Evolution", "E", "Objective Evolution", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Surface> surf = new List<Surface>();
            if (!DA.GetDataList(0, surf)) { return; }
            Surfaces = surf.Select(x=>x.ToNurbsSurface()).ToList();

            double score = 0;
            if (!DA.GetData(1, ref score)) { return; }
            Objective = score;

            List<double> max = new List<double>();
            List<double> min = new List<double>();
            if (!DA.GetDataList(2, max)) { return; }
            if (!DA.GetDataList(3, min)) { return; }
            Max = max;
            Min = min;

            List<Point3d> ptsx = new List<Point3d>();
            DA.GetDataList(4, ptsx);
            FPtsX= ptsx;

            List<Point3d> ptsy = new List<Point3d>();
            DA.GetDataList(5, ptsy);
            FPtsY = ptsy;

            List<Point3d> ptsz = new List<Point3d>();
            DA.GetDataList(6, ptsz);
            FPtsZ = ptsz;

            List<double> constr = new List<double>();
            if (!DA.GetDataList(7, constr)) { return; }
            Constraints = constr;

            DA.SetDataList(0, Evolution);
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new TestSurfConstOptAttributes(this);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{fe8a508d-ff75-423d-b767-c4739236d509}"); }
        }
    }
    public class TestSurfConstOptAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public TestSurfConstOptAttributes(IGH_Component component) : base(component)
        {
            MyComponent = (GuideTestSurfConstOptComponent)component;
        }

        GuideTestSurfConstOptComponent MyComponent;

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick
            (Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            Design design = HelperFunctions.GenerateDesign(MyComponent);
            DialogResult result = MessageBox.Show("Run Optimization?", "Optimization", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes) { design.Optimize(); }
            return base.RespondToMouseDoubleClick(sender, e);
        }
    }
}
