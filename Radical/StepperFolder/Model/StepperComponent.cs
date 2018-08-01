using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;


namespace Stepper
{
    public class StepperComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public StepperComponent()
          : base("Stepper", "Stepper",
              "Steps in the direction of the gradient",
              "DSE", "Simplify")
        {
            this.IsWindowOpen = false;
            this.Iterating = false;

            this.Variables = new List<double>();
            this.Objectives = new List<double>();     
        }

        #region Variables
        //Variables to store user input
        public bool IsWindowOpen;
        public bool Iterating;
        public List<double> Variables;
        public List<double> Objectives;
        public List<List<double>> ObjData;
        #endregion

        public override void CreateAttributes()
        {
            //base.m_attributes = new ComponentAttributes(this);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Variables", "Var", "Variables to be sampled.  Must be less than 13.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Objectives", "Obj", "One or more performance objectives", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //PUT OUTPUTS HERE IF DECIDED UPON
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Store variables to be accessed later from the Window thread

            var vars = new List<double>();
            if (!DA.GetDataList(0, vars)) return;
            this.Variables = vars;

            var objs = new List<double>();
            if (!DA.GetDataList(1, objs)) return;
            this.Objectives = objs;
        }

        #region Data Structures
        static List<List<double>> StructureToListOfLists(GH_Structure<GH_Number> structure)
        {
            List<List<double>> list = new List<List<double>>();
            foreach (GH_Path p in structure.Paths)
            {
                List<GH_Number> l = (List<GH_Number>)structure.get_Branch(p);
                List<double> doubles = new List<double>();
                foreach (GH_Number n in l)
                {
                    double d = 0;
                    n.CastTo<double>(out d);
                    doubles.Add(d);
                }
                list.Add(doubles);
            }
            return list;
        }

        static DataTree<T> ListOfListsToTree<T>(List<List<T>> listofLists)
        {
            DataTree<T> tree = new DataTree<T>();
            for (int i = 0; i < listofLists.Count; i++)
            {
                tree.AddRange(listofLists[i], new GH_Path(i));
            }
            return tree;
        }

        static DataTree<T> ListToTree<T>(List<T> List)
        {
            DataTree<T> tree = new DataTree<T>();
            for (int i = 0; i < List.Count; i++)
            {
                tree.Add(List[i], new GH_Path(i));
            }
            return tree;
        }
        #endregion

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
            get { return new Guid("aff76ef2-a2c5-4a5b-9a68-c6e3644bbff2"); }
        }
    }
}
