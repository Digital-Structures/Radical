using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLoptNet;
using System.Threading;
using System.Threading.Tasks;
using Radical.TestComponents;
using System.Windows.Forms;

namespace Radical.Integration
{
    public class Optimizer

    {
        //static bool verbose = true;
        public double? MinValue; //init objective

        public Optimizer(IDesign design)
        {
            Design = design;
            this.MainAlg = NLoptAlgorithm.LD_LBFGS;
            BuildWrapper();
            SetBounds();
            Solver.SetMinObjective((x) => Objective(x));
            if (Design.Constraints != null)
            {
                List<ConstraintNumber> consts = new List<ConstraintNumber>();
                for (int i = 0; i < Design.Constraints.Count; i++)
                {
                    consts.Add(new ConstraintNumber(i));
                }
                foreach (ConstraintNumber c in consts)
                {
                    Solver.AddLessOrEqualZeroConstraint((x) => Constraint(x, c));
                }
            }
        }

        public Optimizer(IDesign design, RadicalWindow radicalWindow)
        {
            Design = design;
            this.MainAlg = radicalWindow.RadicalVM.PrimaryAlgorithm;
            //this.SecondaryAlg = NLoptAlgorithm.LN_COBYLA;
            this.RadicalWindow = radicalWindow;
            BuildWrapper();
            SetBounds();
            Solver.SetMinObjective((x) => Objective(x));
            if (Design.Constraints != null)
            {
                List<ConstraintNumber> consts = new List<ConstraintNumber>();
                for (int i = 0; i < Design.Constraints.Count; i++)
                {
                    consts.Add(new ConstraintNumber(i));
                }
                foreach (ConstraintNumber c in consts)
                {
                    Solver.AddLessOrEqualZeroConstraint((x) => Constraint(x, c));
                }
            }
        }

        public Optimizer(IDesign design, double relStopTol, int niter)
        {
            Design = design;
            this.MainAlg = NLoptAlgorithm.LN_COBYLA;
            BuildWrapper(relStopTol, niter);
            SetBounds();
            Solver.SetMinObjective((x) => Objective(x));
            if (Design.Constraints != null)
            {
                for (int i = 0; i < Design.Constraints.Count; i++)
                {
                    Solver.AddLessOrEqualZeroConstraint((x) => Constraint(x, i));
                }
            }
        }

        public IDesign Design;
        public uint nVars { get { return (uint)this.Design.Variables.Count; } }

        public NLoptAlgorithm MainAlg;
        public NLoptAlgorithm SecondaryAlg; // Optional
        public NLoptSolver Solver;
        public RadicalWindow RadicalWindow;

        public void BuildWrapper()
        {
            this.Solver = new NLoptSolver(MainAlg, nVars, this.RadicalWindow.RadicalVM.ConvCrit, this.RadicalWindow.RadicalVM.Niterations);
        }
        public void BuildWrapper(double relStopTol, int niter)
        {
            this.Solver = new NLoptSolver(MainAlg, nVars, relStopTol, niter);
        } // relStopTO, niter should be made optional arguments

        public void SetBounds()
        {
            Solver.SetLowerBounds(Design.Variables.Select(x => x.Min).ToArray());
            Solver.SetUpperBounds(Design.Variables.Select(x => x.Max).ToArray());
        }

        public double Objective(double[] x)
        {


            bool finished = false;
            Grasshopper.Kernel.GH_SolutionMode refresh = Grasshopper.Kernel.GH_SolutionMode.Silent;

            if (this.RadicalWindow.RadicalVM.Mode == RefreshMode.Live) { refresh = Grasshopper.Kernel.GH_SolutionMode.Default; }

            System.Action run = delegate ()
            {
                //Grasshopper.Instances.ActiveCanvas.Enabled = false;

                for (int i = 0; i < nVars; i++)
                {
                    IVariable var = Design.Variables[i];
                    var.UpdateValue(x[i]);
                }
                Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true, refresh);
                finished = true;
            };
            Rhino.RhinoApp.MainApplicationWindow.Invoke(run);


            // inelegant, temporary
            while (!finished)
            {
                Thread.Sleep(1);
            }

            double objective = Design.CurrentScore;
            Design.ScoreEvolution.Add(objective);
            ((Design)Design).OptComponent.Evolution = Design.ScoreEvolution;
            if (this.RadicalWindow.RadicalVM.Mode != RefreshMode.Silent) { this.RadicalWindow.UpdateWindow(Design.ScoreEvolution); }

            try
            {
                this.RadicalWindow.source.Token.ThrowIfCancellationRequested();
            }
            catch
            {
                throw;
            }

            return objective;
        }

        public double Objective(double[] x, ref double[] grad)
        {
            for (int i = 0; i < nVars; i++)
            {
                IVariable var = Design.Variables[i];
                var.UpdateValue(x[i]);
            }

            if (grad != null) { }//update gradient, for gradient-based algs.

            double objective = Design.CurrentScore;
            Design.ScoreEvolution.Add(objective);
            return objective;
        } //UNIMPLEMENTED

        public double Constraint(double[] x, Object data)
        {
            ConstraintNumber d = (ConstraintNumber)data;
            int num = d.i;
            for (int i = 0; i < nVars; i++)
            {
                IVariable var = Design.Variables[i];
                var.UpdateValue(x[i]);
            }
            foreach (IDesignGeometry vargeo in Design.Geometries)
            {
                vargeo.Update();
            }
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
            return Design.Constraints[num].CurrentValue;
        }

        public NloptResult RunOptimization()
        {
            double[] x = Design.Variables.Select(t => t.CurrentValue).ToArray();
            return Solver.Optimize(x, out MinValue);
        }
    }

    public struct ConstraintNumber
    {
        public int i;
        public ConstraintNumber(int i)
        {
            this.i = i;
        }
    }



}
