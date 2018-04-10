using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLoptNet;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Radical.TestComponents
{
    public class GuideTestComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GuideTestComponent()
          : base("GuideTest", "Guide",
              "Guide",
              "Test", "Guide")
        {
        }

        public Surface OriginalSurface { get; set; }
        public NurbsSurface MySurface { get; set; }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Srf", "S", "surface", GH_ParamAccess.item);
            //pManager.AddSurfaceParameter("Srf", "S", "surface", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Srf", "S", "surface", GH_ParamAccess.item);
            
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new TestAttributes(this);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface srf = null;

            if (!DA.GetData(0, ref srf)) { return; }
            //DA.GetData(1, ref srf2);
            MySurface = srf.ToNurbsSurface();
            MySurface.Points.SetControlPoint(0, 0, new ControlPoint(0, 0, 10));

            DA.SetData(0, MySurface);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{e6b1d927-e536-42cb-a431-08ec1b388818}"); }
        }
    }


    public class TestAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public TestAttributes(IGH_Component component) : base(component)
        {
            MyComponent = (GuideTestComponent)component;
        }

        GuideTestComponent MyComponent;

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick
            (Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {

            this.MyComponent.Params.Input[0].Sources[0].RemoveAllSources();
            var srf = this.MyComponent.Params.Input[0].Sources[0] as Grasshopper.Kernel.GH_PersistentGeometryParam<Grasshopper.Kernel.Types.GH_Surface>;
            //var param  = this.MyComponent.Params.Input[0].Sources[0] as Grasshopper.Kernel.GH_PersistentParam<Grasshopper.Kernel.Types.GH_Surface>;
            //param.PersistentData.Clear();
            srf.PersistentData.Clear();
            srf.PersistentData.Append(new Grasshopper.Kernel.Types.GH_Surface(this.MyComponent.MySurface));

            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
            return base.RespondToMouseDoubleClick(sender, e);
        }
    }
}
