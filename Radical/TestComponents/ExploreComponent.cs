using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Radical.Components;
using Radical.Integration;

namespace Radical.TestComponents
{
    public class ExploreComponent : GH_Component, IExplorationComponent
    {
        /// <summary>
        /// Initializes a new instance of the ExploreComponent class.
        /// </summary>
        public ExploreComponent()
          : base("Explore", "Exp",
              "Guide",
              "Test", "Guide")
        {
            dVars = new List<double>();
            dProp = new List<double>();
        }
        public List<double> dVars { get; set; }
        public List<double> dProp { get; set; }
        public int nSamples { get; set; }
        public string path { get; set; }
        public string filename { get; set; }
        public int algorithm { get; set; }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Variables", "Vars", "Variables", GH_ParamAccess.list);
            pManager.AddNumberParameter("Properties", "Props", "Properties to Record", GH_ParamAccess.list);
            pManager.AddNumberParameter("nSamples", "n", "Number of Samples to Record", GH_ParamAccess.item);
            pManager.AddTextParameter("Path", "P", "Path where to save sampling data", GH_ParamAccess.item);
            pManager.AddTextParameter("Filename", "F", "Name of sampling data file", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Algorithm", "Alg", "Sampling algorithm", GH_ParamAccess.item, 2);
            //pManager.AddIntegerParameter("Seed", "s", "Seed for random processes", GH_ParamAccess.item, 0);
            pManager[1].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Sampled Variables", "sVars", "Sampled Variables", GH_ParamAccess.tree);
            pManager.AddNumberParameter("Sampled Properties", "sProps", "Sampled Properties", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            dProp = new List<double>();
            dVars = new List<double>();
            double n = 0;
            string p = null;
            string f = null;
            int alg = 0;
            if (!DA.GetDataList(0, dVars)) { return; };
            if (!DA.GetDataList(1, dProp)) { return; };
            if (!DA.GetData(2, ref n)) { return; };
            if (!DA.GetData(3, ref p)) { return; };
            if (!DA.GetData(4, ref f)) { return; };
            DA.GetData(5, ref alg);
            filename = f;
            path = p;
            nSamples = (int)n;
            algorithm = alg;
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

        public override void CreateAttributes()
        {
            base.m_attributes = new ExploreComponentAttributes(this);
        }

        public class ExploreComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
        {
            public ExploreComponentAttributes(IGH_Component component) : base(component)
            {
                MyComponent = (ExploreComponent)component;
            }

            ExploreComponent MyComponent;

            [STAThread]
            public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick
                (Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
            {
                Design design = HelperFunctions.GenerateDesign(MyComponent);

                DialogResult result = MessageBox.Show("Run Sampling?", "Sampling", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes) { design.Sample(MyComponent.algorithm); }
                if (MyComponent.path != null || MyComponent.filename != null)
                {
                    Radical.Integration.csvWriter.CreateSamplingRecord(MyComponent.path, MyComponent.filename, design);
                }
                return base.RespondToMouseDoubleClick(sender, e);
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("423f026d-41bf-4b0e-b0c8-43f469dbb5d4"); }
        }

    }
}