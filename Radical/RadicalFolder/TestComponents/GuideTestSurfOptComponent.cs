using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Radical.Integration;
using Radical.Components;

namespace Radical.TestComponents
{
    public class GuideTestSurfOptComponent : GH_Component,IOptimizationComponent
    {

        public double Objective { get; set; }
        public List<double> Objectives { get; set; }
        public List<double> Constraints { get; set; }
        public List<double> Evolution { get; set; }

        public NurbsSurface Surface;
        public double Min=0;
        public double Max = 0;
        public List<Point3d> FPtsX = new List<Point3d>();
        public List<Point3d> FPtsY = new List<Point3d>();
        public List<Point3d> FPtsZ = new List<Point3d>();
        

        /// <summary>
        /// Initializes a new instance of the GuideTestSurfOptComponent class.
        /// </summary>
        public GuideTestSurfOptComponent()
          : base("GuideTestSurfOptComponent", "Guide",
              "Guide",
              "Test", "Guide")
        {
            Objective = 0;
            Evolution = new List<double>();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Variable Surface", "VarSrf", "Design Surface", GH_ParamAccess.item);
            pManager.AddNumberParameter("Objective", "O", "Objective to Minimize", GH_ParamAccess.item);
            pManager.AddNumberParameter("Upper Bound", "uB", "Upper Bound for Design Variables", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lower Bound", "lB", "Lower Bound for Design Variables", GH_ParamAccess.item);
            pManager.AddPointParameter("Fixed X Points", "pX", "Points with X Position Fixed during Optimization", GH_ParamAccess.list);
            pManager.AddPointParameter("Fixed Y Points", "pY", "Points with Y Position Fixed during Optimization", GH_ParamAccess.list);
            pManager.AddPointParameter("Fixed Z Points", "pZ", "Points with Z Position Fixed during Optimization", GH_ParamAccess.list);
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
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

            Surface surf = null;
            if (!DA.GetData(0, ref surf)) { return; }
            Surface = surf.ToNurbsSurface();

            double score = 0;
            if (!DA.GetData(1, ref score)) { return; }
            Objective = score;

            if (!DA.GetData(2, ref Max)) { return; }
            if (!DA.GetData(3, ref Min)) { return; }
            if (!DA.GetDataList(4, FPtsX)) { return; }
            if (!DA.GetDataList(5, FPtsY)) { return; }
            if (!DA.GetDataList(6, FPtsZ)) { return; }

            DA.SetDataList(0, Evolution);
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new TestSurfOptAttributes(this);
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
            get { return new Guid("{b7fb63e3-dbe3-4809-a230-d7ba7ab5c2e8}"); }
        }
    }
    public class TestSurfOptAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public TestSurfOptAttributes(IGH_Component component) : base(component)
        {
            MyComponent = (GuideTestSurfOptComponent)component;
        }

        GuideTestSurfOptComponent MyComponent;

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