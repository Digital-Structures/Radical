﻿using System;
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

namespace Radical.TestComponents
{
    public class RadicalComponent : GH_Component, IOptimizationComponent
    {
        /// <summary>
        /// Initializes a new instance of the RadicalComponent class.
        /// </summary>
        public RadicalComponent()
          : base("Radical", "Rad",
              "Optimization Component",
              "DSE", "Optimization")
        {
            Objective = 0;
            this.NumVariables = new List<double>();
            this.SrfVariables = new List<NurbsSurface>();
            this.CrvVariables = new List<Curve>();
            this.Constraints = new List<double>();
        }

        public double Objective { get; set; }
        public List<double> Constraints { get; set; }
        public List<double> NumVariables { get; set; }
        public List<NurbsSurface> SrfVariables { get; set; }
        public List<Curve> CrvVariables { get; set; }


        public List<double> Evolution { get; set; }

        public override void CreateAttributes()
        {
            base.m_attributes = new RadicalComponentAttributes(this);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Objective", "O", "Objective to Minimize", GH_ParamAccess.item);
            pManager.AddNumberParameter("Constraints", "C", "Optimization Constraints", GH_ParamAccess.list);
            pManager.AddNumberParameter("Numerical Variables", "numVar", "Numerical Optimization Variables", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Variable Surfaces", "srfVar", "Geometrical Optimization Variables (Surfaces)", GH_ParamAccess.list);
            pManager.AddSurfaceParameter("Variable Curves", "crvVar", "Geometrical Optimization Variables (Curves)", GH_ParamAccess.list);
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
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //assign objective
            double obj = 0;
            if (!DA.GetData(0,ref obj)) { return; }
            this.Objective = obj;
            //assign constraints
            List<double> constraints = new List<double>();
            DA.GetDataList(1, constraints);
            this.Constraints = constraints;
            //assign numerical variables
            List<double> variables = new List<double>();
            if (!DA.GetDataList(2, variables)) { return; }
            this.NumVariables = variables;
            //assign surface variables
            List<Surface> surfaces= new List<Surface>();
            if (!DA.GetDataList(3, surfaces)) { return; }
            this.SrfVariables = surfaces.Select(x=>x.ToNurbsSurface()).ToList();
            //assign curve variables
            List<Curve> curves = new List<Curve>();
            if (!DA.GetDataList(4, curves)) { return; }
            this.CrvVariables = curves;
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


    public class RadicalComponentAttributes : Grasshopper.Kernel.Attributes.GH_ComponentAttributes
    {
        // custom attribute to override double click mouse event on component and open a WPF window

        public RadicalComponentAttributes(IGH_Component component) : base(component)
        {
            MyComponent = (RadicalComponent)component;
        }

        RadicalComponent MyComponent;

        //[STAThread]
        public override Grasshopper.GUI.Canvas.GH_ObjectResponse RespondToMouseDoubleClick(Grasshopper.GUI.Canvas.GH_Canvas sender, Grasshopper.GUI.GH_CanvasMouseEvent e)
        {
            Design design = HelperFunctions.GenerateDesign(MyComponent);
            RadicalVM radicalVM = new RadicalVM(design);

            Thread viewerThread = new Thread(delegate ()
            {
                Window viewer = new Radical.RadicalWindow(radicalVM);
                viewer.Show();
                //viewer.Topmost = true;
                System.Windows.Threading.Dispatcher.Run();
            });

            viewerThread.SetApartmentState(ApartmentState.STA); // needs to be STA or throws exception
            viewerThread.Start();
            return base.RespondToMouseDoubleClick(sender, e);
        }
    }
}