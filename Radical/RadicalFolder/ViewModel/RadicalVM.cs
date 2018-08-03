using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Radical.Integration;
using NLoptNet;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;
using DSOptimization;
using LiveCharts;

namespace Radical
{
    public class RadicalVM : BaseVM
    {
        private List<GroupVarVM> GroupVars;
        private DSOptimizerComponent Component;
        public Design Design;
        public List<ConstVM> Constraints;
        public List<VarVM> NumVars;
        public List<List<VarVM>> GeoVars;
        public Dictionary<string, List<GraphVM>> Graphs;
        public enum Direction { X, Y, Z };

        //EVOLUTIONS for all objectives and constraints 
        public ChartValues<double> ObjectiveEvolution { get; set; }
        public ChartValues<ChartValues<double>> ConstraintsEvolution { get; set; }

        public RadicalVM()
        {
        }

        public RadicalVM(DSOptimizerComponent component)
        {
            this.Component = component;
        }

        //CONSTRUCTOR
        public RadicalVM(Design design, DSOptimizerComponent component)
        {
            this.Component = component;
            this.Design = design;

            this.Constraints = new List<ConstVM>();

            this.ObjectiveEvolution = new ChartValues<double>();
            this.ConstraintsEvolution = new ChartValues<ChartValues<double>>();
            foreach (Constraint c in this.Design.Constraints)
            {
                this.ConstraintsEvolution.Add(new ChartValues<double>());
            }

            this.Graphs = new Dictionary<string, List<GraphVM>>();
            this.Graphs.Add("Main", new List<GraphVM>());
            this.Graphs.Add("Constraints", new List<GraphVM>());
            SetUpGraphs();

            this.NumVars = new List<VarVM> { };
            this.GeoVars = new List<List<VarVM>> { };
            this.GroupVars = new List<GroupVarVM> { };
            SortVariables();

            this.OptRunning = false;
            this.OptRunning = false;
            this._advancedOptions = false;
        }

        //ACTIVE GRAPHS
        //A list of active constraint graphs to populate the window graph grid
        public List<GraphVM> ActiveGraphs
        {
            get
            {
                List<GraphVM> list = new List<GraphVM>();
                list.Add(this.Graphs["Main"][0]);

                foreach (ConstVM c in Constraints.Where(c => c.IsActive))
                    list.Add(c.GraphVM);

                return list;
            }
        }

        //SET UP GRAPHS
        public void SetUpGraphs()
        {
            GraphVM main = new GraphVM(this.ObjectiveEvolution, "Objective");
            this.Graphs["Main"].Add(main);

            for (int i = 0; i < Design.Constraints.Count; i++)
            {
                GraphVM gvm = new GraphVM(ConstraintsEvolution[i], String.Format("C{0}", i));
                this.Graphs["Constraints"].Add(gvm);
                this.Constraints.Add(new ConstVM(Design.Constraints[i], gvm));
            }
        }

        public void UpdateGraphLines(int iteration)
        {
            foreach (GraphVM graph in this.ActiveGraphs)
            {
                graph.UpdateLine(iteration);
            }
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
                    geoVar.Name += ((GeoVariable)geoVar.DesignVar).PointName;

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

            foreach (ConstVM constraint in this.Constraints)
                constraint.ChangesEnabled = false;
        }

        //OPTIMIZATION FINISHED
        //Enable changes to all optimization variables and constraints
        public void OptimizationFinished()
        {
            this.ChangesEnabled = true;

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

        //GRAPH COLUMNS
        private int _cols;
        public int Cols
        {
            get
            {
                return _cols;
            }
            set
            {
                if(CheckPropertyChanged<int>("Cols", ref _cols, ref value))
                {
                    foreach (GraphVM gvm in ActiveGraphs)
                    {
                        gvm.ChartRowVisibility = Visibility.Collapsed;
                        gvm.ChartRowVisibility = Visibility.Visible;
                    }
                }
            }
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

        #region Algorithms 
        //PRIMARY ALGORITHM
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
                if (AdvancedOptions)
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
                else
                {
                    if (this.Constraints.Any())
                    {
                        return BasicAlgs_INEQ.ToList();
                    }
                    else
                    {
                        return BasicAlgs.ToList();
                    }
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

        //ADVANCED OPTIONS
        private bool _advancedOptions;
        public bool AdvancedOptions
        {
            get { return _advancedOptions; }
            set
            {
                if (CheckPropertyChanged<bool>("AdvancedOptions", ref _advancedOptions, ref value))
                {
                    FirePropertyChanged("AvailableAlgs");
                }
            }
        }

        public IEnumerable<NLoptAlgorithm> BasicAlgs = new[]
        {
            NLoptAlgorithm.AUGLAG, //Calls for secondary alg
            NLoptAlgorithm.LN_COBYLA,
        };

        public IEnumerable<NLoptAlgorithm> BasicAlgs_EQ = new[]
        {
            NLoptAlgorithm.LN_COBYLA
        };

        public IEnumerable<NLoptAlgorithm> BasicAlgs_INEQ = new[]
        {
            NLoptAlgorithm.LN_COBYLA
        };

        //List of those that require secondary and the secondary options remain the same. 

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
        #endregion

        public void Optimize(RadicalWindow radicalWindow)
        {
            RadicalOptimizer opt = new RadicalOptimizer(this.Design, radicalWindow);
            opt.RunOptimization();
        }
    }
}
