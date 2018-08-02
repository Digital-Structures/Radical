using System;
using System.Collections.Generic;
using Radical;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using Radical.Components;
using Radical.Integration;
using System.Threading;
using Stepper;

namespace DSOptimization
{ 
    public class DSOptimizerComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the RadicalComponent class.
        /// </summary>
        public DSOptimizerComponent()
          : base("DS Optimizer", "DSOpt",
              "Optimization Component",
              "DSE", "Optimization")
        {
            this.Objectives = new List<double>();

            this.NumVariables = new List<double>();
            this.SrfVariables = new List<NurbsSurface>();
            this.CrvVariables = new List<NurbsCurve>();

            this.Constraints = new List<double>();

            this.open = false; //Is window open
        }

        public List<double> Objectives { get; set; }
        public List<double> Constraints { get; set; }
        public List<double> NumVariables { get; set; }
        public List<NurbsSurface> SrfVariables { get; set; }
        public List<NurbsCurve> CrvVariables { get; set; }

        //Determine whether there is already a Radical window open
        private bool open;
        public bool IsWindowOpen
        {
            get { return this.open; }
            set { this.open = value; }
        }

        public override void CreateAttributes()
        {
            base.m_attributes = new DSOptimizationComponentAttributes(this);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Objective", "O", "Objective to Minimize", GH_ParamAccess.list);
            pManager.AddNumberParameter("Constraints", "C", "Optimization Constraints", GH_ParamAccess.list);
            pManager.AddNumberParameter("Numerical Variables", "numVar", "Numerical Optimization Variables", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Variable Surfaces", "srfVar", "Geometrical Optimization Variables (Surfaces)", GH_ParamAccess.list);
            pManager.AddCurveParameter("Variable Curves", "crvVar", "Geometrical Optimization Variables (Curves)", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// Stores variables to be accessed later from the Window thread
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //assign objective
            List<double> obj = new List<double>();
            if (!DA.GetDataList(0, obj)) { return; }
            this.Objectives = obj;

            //assign constraints
            List<double> constraints = new List<double>();
            DA.GetDataList(1, constraints);
            this.Constraints = constraints;

            //assign numerical variables
            List<double> variables = new List<double>();
            DA.GetDataList(2, variables);
            this.NumVariables = variables;

            //assign surface variables
            List<Surface> surfaces= new List<Surface>();
            DA.GetDataList(3, surfaces);
            this.SrfVariables = surfaces.Select(x=>x.ToNurbsSurface()).ToList();

            //assign curve variables
            List<Curve> curves = new List<Curve>();
            DA.GetDataList(4, curves);
            this.CrvVariables = curves.Select(x => x.ToNurbsCurve()).ToList();
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
            get { return new Guid("65ec1771-a3e9-4cba-946e-c7b3ed26d98a"); }
        }
    }


    public class DSOptimizationComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        // custom attribute to override double click mouse event on component and open a WPF window

        DSOptimizerComponent MyComponent;

        public DSOptimizationComponentAttributes(IGH_Component component) : base(component)
        {
            MyComponent = (DSOptimizerComponent)component;
        }

        //[STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            //Prevent opening of multiple windows at once
            if (!MyComponent.IsWindowOpen)
            {
                MyComponent.IsWindowOpen = true;

                //Would be nice to not need the HelperFunctions file
                Design design = new Design(MyComponent);

                RadicalVM radicalVM = new RadicalVM(design, this.MyComponent);
                //StepperVM stepperVM = new StepperVM(this.MyComponent, design);

                Thread viewerThread = new Thread(delegate ()
                {
                    Window viewer = new Radical.RadicalWindow(radicalVM);

                    //Window viewer = new StepperWindow(stepperVM);

                    viewer.Show();
                    System.Windows.Threading.Dispatcher.Run();
                });

                viewerThread.SetApartmentState(ApartmentState.STA); // needs to be STA or throws exception
                viewerThread.Start();
            }
            return base.RespondToMouseDoubleClick(sender, e);
        }
        
    }
}