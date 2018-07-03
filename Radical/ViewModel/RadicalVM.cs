using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical.TestComponents;
using Radical.Integration;
using NLoptNet;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;


namespace Radical
{
    public class RadicalVM : BaseVM
    {
        private RadicalComponent Component;
        public IDesign Design;
        public List<ConstVM> Constraints;
        public List<VarVM> NumVars;
        public List<List<VarVM>> GeoVars;
        public enum Direction { X, Y, Z };

        public RadicalVM()
        {
        }

        public RadicalVM(RadicalComponent component)
        {
            this.Component = component;
        }

        //CONSTRUCTOR
        public RadicalVM(IDesign design, RadicalComponent component)
        {
            this.Component = component;
            this.Design = design;

            if (Design.Constraints != null)
            {
                Constraints = this.Design.Constraints.Select(x => new ConstVM(x)).ToList();
            }

            this.NumVars = new List<VarVM> { };
            this.GeoVars = new List<List<VarVM>> { };
            SortVariables();

            this.OptRunning = false;
        }

        //SORT VARIABLES
        //Separate geometric and numeric variables
        //Sorting helps with UI stack panel organization
        private void SortVariables()
        {
            //GEOMETRIES
            int geoIndex = 1;
            foreach (IDesignGeometry geo in this.Design.Geometries)
            {
                List<VarVM> singleGeoVars = new List<VarVM> { };

                //Add all the variables for that geometry to a sublist of varVMs
                int varIndex = 0;
                foreach (GeoVariable var in geo.Variables)
                {
                    VarVM geoVar = new VarVM(var);
                    int dir = var.Dir;

                    //Logical default naming of variable
                    //e.g. G1.u1v1.X
                    geoVar.Name = String.Format("G{0}.", geoIndex) + ((GeoVariable)geoVar.DesignVar).PointName;

                    singleGeoVars.Add(geoVar);
                    varIndex++;
                }

                this.GeoVars.Add(singleGeoVars);
                geoIndex++;
            }

            //SLIDERS
            /***This is probably not the best way to do this as it involves looping over geometry variables already stored***/
            foreach (var numVar in this.Design.Variables.Where(numVar => numVar is SliderVariable))
            {
                this.NumVars.Add(new VarVM(numVar));
            }
        }

        //OPTIMIZATION STARTED
        //Disable changes to all optimization variables and constraints
        public void OptimizationStarted()
        {
            this.ChangesEnabled = false;

            foreach (VarVM var in this.NumVars)
                var.ChangesEnabled = false;
            for (int i = 0; i < this.GeoVars.Count; i++)
                foreach (VarVM var in this.GeoVars[i])
                    var.ChangesEnabled = false;
            foreach (ConstVM constraint in this.Constraints)
                constraint.ChangesEnabled = false;
        }

        //OPTIMIZATION FINISHED
        //Enable changes to all optimization variables and constraints
        public void OptimizationFinished()
        {
            this.ChangesEnabled = true;

            foreach (VarVM var in this.NumVars)
                var.OptimizationFinished();
            for (int i = 0; i < this.GeoVars.Count; i++)
                foreach (VarVM var in this.GeoVars[i])
                    var.OptimizationFinished();
            foreach (ConstVM constraint in this.Constraints)
                constraint.OptimizationFinished();
        }

        //ON WINDOW CLOSING
        //Alert the component that the window has been closed
        //(and therefore a new window can open on double click)
        public void OnWindowClosing()
        {
            this.Component.IsWindowOpen = false;
        }

        //REFRESH MODE
        private RefreshMode _mode;
        public RefreshMode Mode
        {
            get
            { return _mode; }
            set
            {
                if (CheckPropertyChanged<RefreshMode>("Mode", ref _mode, ref value))
                {
                }
            }
        }

        //PRIMMARY ALGORITHM
        private NLoptAlgorithm _primaryalgorithm;
        public NLoptAlgorithm PrimaryAlgorithm
        {
            get
            { return _primaryalgorithm; }
            set
            {
                if (CheckPropertyChanged<NLoptAlgorithm>("PrimaryAlgorithm", ref _primaryalgorithm, ref value))
                {
                }
            }
        }

        //SECONDARY ALGORITHM
        private NLoptAlgorithm _secondaryalgorithm;
        public NLoptAlgorithm SecondaryAlgorithm
        {
            get
            { return _secondaryalgorithm; }
            set
            {
                if (CheckPropertyChanged<NLoptAlgorithm>("SecondaryAlgorithm", ref _secondaryalgorithm, ref value))
                {
                }
            }

        }

        //AVAILABLE ALGORITHMS
        public List<NLoptAlgorithm> AvailableAlgs
        {
            get
            {
                if (this.Constraints.Any())
                {
                    return DFreeAlgs_INEQ.ToList();
                }
                else
                {
                    return DFreeAlgs.ToList();
                }
            }
        }

        //AVAILABLE SECONDARY ALGORITHMS
        public List<NLoptAlgorithm> AvailableSecondaryAlgs
        {
            get
            {
                return DFreeAlgs_Secondary.ToList();
            }
        }

        //NUMBER OF ITERATIONS
        private int _niterations;
        public int Niterations
        {
            get
            { return _niterations; }
            set
            {
                if (CheckPropertyChanged<int>("Niterations", ref _niterations, ref value))
                {
                }
            }

        }

        //CONVERGENCE
        private double _convcrit;
        public double ConvCrit
        {
            get
            { return _convcrit; }
            set
            {
                if (CheckPropertyChanged<double>("ConvCrit", ref _convcrit, ref value))
                {
                }
            }
        }

        public IEnumerable<NLoptAlgorithm> DFreeAlgs = new[]
        {
            NLoptAlgorithm.AUGLAG, //Calls for secondary alg
            NLoptAlgorithm.AUGLAG_EQ, //Calls for secondary alg
            NLoptAlgorithm.GN_CRS2_LM,
            NLoptAlgorithm.GN_DIRECT,
            NLoptAlgorithm.GN_DIRECT_L,
            NLoptAlgorithm.GN_DIRECT_L_NOSCAL,
            NLoptAlgorithm.GN_DIRECT_L_RAND,
            NLoptAlgorithm.GN_DIRECT_L_RAND_NOSCAL,
            NLoptAlgorithm.GN_DIRECT_NOSCAL,
            NLoptAlgorithm.GN_ESCH,
            NLoptAlgorithm.GN_ISRES,
            NLoptAlgorithm.GN_ORIG_DIRECT,
            NLoptAlgorithm.GN_ORIG_DIRECT_L,
            NLoptAlgorithm.G_MLSL, //calls for secondary alg (local)
            NLoptAlgorithm.G_MLSL_LDS, //calls for secondary alg (local)
            NLoptAlgorithm.LN_BOBYQA,
            NLoptAlgorithm.LN_COBYLA,
            //NLoptAlgorithm.LN_NELDERMEAD, // supersed by SBPLX
            //NLoptAlgorithm.LN_NEWUOA_BOUND,// superseded by BOBYQA
            //NLoptAlgorithm.LN_PRAXIS, // COBYLA OR BOBYQA better
            NLoptAlgorithm.LN_SBPLX
        };

        public IEnumerable<NLoptAlgorithm> DFreeAlgs_EQ = new[]
        {
            NLoptAlgorithm.AUGLAG, //Calls for secondary alg
            NLoptAlgorithm.AUGLAG_EQ, //Calls for secondary alg
            NLoptAlgorithm.GN_ISRES,
            NLoptAlgorithm.LN_COBYLA
        };

        public IEnumerable<NLoptAlgorithm> DFreeAlgs_INEQ = new[]
        {
            NLoptAlgorithm.AUGLAG, //Calls for secondary alg
            NLoptAlgorithm.AUGLAG_EQ, //Calls for secondary algorithm that can handle inequality constraints
            NLoptAlgorithm.GN_ISRES,
            NLoptAlgorithm.GN_ORIG_DIRECT,
            NLoptAlgorithm.GN_ORIG_DIRECT_L,
            NLoptAlgorithm.LN_COBYLA
        };


        //Algorithms that require a secondary algorithm 
        public IEnumerable<NLoptAlgorithm> DFreeAlgs_ReqSec = new[]
        {
            NLoptAlgorithm.AUGLAG, //Calls for secondary alg
            NLoptAlgorithm.AUGLAG_EQ, //Calls for secondary alg
            NLoptAlgorithm.G_MLSL, //calls for secondary alg (local)
            NLoptAlgorithm.G_MLSL_LDS, //calls for secondary alg (local)
        };

        public IEnumerable<NLoptAlgorithm> DFreeAlgs_Secondary = new[]
        {
            NLoptAlgorithm.LN_BOBYQA,
            NLoptAlgorithm.LN_COBYLA,
        };
    }
}
