using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Radical.Integration;
using Radical.Components;


namespace Radical.TestComponents
{
    public class GuideTestOptComponent : GH_Component,IOptimizationComponent 
    {
        /// <summary>
        /// Initializes a new instance of the GuideTestOptComponent class.
        /// </summary>.
        /// 


        public double Objective { get; set; }
        public List<double> Objectives { get; set; }
        public List<double> Constraints { get; set; }
        public List<double> Evolution { get; set; }


        public GuideTestOptComponent()
          : base("GuideTest", "Guide",
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
            pManager.AddNumberParameter("Variables", "Var", "Design Variables", GH_ParamAccess.list);
            pManager.AddNumberParameter("Objective", "O", "Objective to Minimize", GH_ParamAccess.item);
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
            double score = 0;
            if(!DA.GetData(1,ref score)) { return; }
            Objective = score;
            DA.SetDataList(0, Evolution);
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new TestOptAttributes(this);
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
            get { return new Guid("{fde4adab-cd79-492e-bbf1-5c421f743f14}"); }
        }

    }

    public class TestOptAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        public TestOptAttributes(IGH_Component component) : base(component)
        {
            MyComponent = (GuideTestOptComponent)component;
        }

        GuideTestOptComponent MyComponent;

        [STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick
            (Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            Design design = HelperFunctions.GenerateDesign(MyComponent);
            DialogResult result = MessageBox.Show("Run Optimization?","Optimization", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes) { design.Optimize(); }
            return base.RespondToMouseDoubleClick(sender, e);
        }
    }
}