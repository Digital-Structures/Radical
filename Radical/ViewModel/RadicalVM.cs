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
        public RadicalVM()
        {
        }

        public RadicalVM(RadicalComponent component)
        {
            this.Component = component;
        }

        public RadicalVM(IDesign design)
        {
            this.Design = design;
            if (Design.Constraints != null)
            {
                Constraints = this.Design.Constraints.Select(x => new ConstVM(x)).ToList();

            }
            Variables = this.Design.Variables.Select(x => new VarVM(x)).ToList();
        }

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

        private RadicalComponent Component;
        public IDesign Design;
        public List<ConstVM> Constraints;
        public List<VarVM> Variables;

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

    }


}
