using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLoptNet;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using DSOptimization;

namespace Radical
{
    //RADICAL OPTIMIZER 
    public class RadicalOptimizer

    {
        //static bool verbose = true;
        public double? MinValue; //init objective

        // CONSTRUCTOR FOR RADICAL
        public RadicalOptimizer(Design design, RadicalWindow radicalWindow)
        {
            this.Design = design;
            this.MainAlg = radicalWindow.RadicalVM.PrimaryAlgorithm;
            //this.SecondaryAlg = NLoptAlgorithm.LN_COBYLA;
            this.RadicalWindow = radicalWindow;
            BuildWrapper();
            SetBounds();
            Solver.SetMinObjective((x) => Objective(x));

            StoredMainValues = new ChartValues<double>();
            StoredConstraintValues = new ChartValues<ChartValues<double>>();

            if (Design.Constraints != null)
            {
                foreach (Constraint c in Design.Constraints)
                {
                    if (c.IsActive)
                    {
                        StoredConstraintValues.Add(new ChartValues<double>());
                        if (c.MyType == Constraint.ConstraintType.lessthan)
                            Solver.AddLessOrEqualZeroConstraint((x) => constraint(x, c));
                        else if (c.MyType == Constraint.ConstraintType.morethan)
                            Solver.AddLessOrEqualZeroConstraint((x) => -constraint(x, c));
                        else
                            Solver.AddEqualZeroConstraint((x) => constraint(x, c));
                    }
                }
            }
        }

        public Design Design;
        public uint nVars { get { return (uint)this.Design.ActiveVariables.Count; } }

        public NLoptAlgorithm MainAlg;
        public NLoptAlgorithm SecondaryAlg; // Optional
        public NLoptSolver Solver;
        public RadicalWindow RadicalWindow;

        public ChartValues<double> StoredMainValues;
        public ChartValues<ChartValues<double>> StoredConstraintValues;

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
                    IVariable var = Design.ActiveVariables[i]; // ONLY INCL. VARS
                    var.UpdateValue(x[i]);
                }
                // Once all points have been updated, we can update the geometries
                foreach (IDesignGeometry geo in this.Design.Geometries)
                {
                    geo.Update();
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

            double objective = Design.Objectives[0];

            //If RefreshMode == Silent then values are stored in a local list so that the graph is not updated
            if (this.RadicalWindow.RadicalVM.Mode == RefreshMode.Silent)
            {
                this.StoredMainValues.Add(objective);

                for (int i = 0; i < Design.ConstraintEvolution.Count; i++)
                {
                    double score = Design.Constraints[i].CurrentValue;
                    this.StoredConstraintValues[i].Add(score);
                }
            }
            else
            {
                //If refresh mode is switched from Silent to Live 
                if (StoredMainValues.Any())
                {
                    AppendStoredValues();
                }

                //Adds main objective values to list and draws
                Design.ScoreEvolution.Add(objective);

                for (int i = 0; i < Design.ConstraintEvolution.Count; i++)
                {
                    double score = Design.Constraints[i].CurrentValue;
                    Design.ConstraintEvolution[i].Add(score);
                }
            }

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

        public NloptResult RunOptimization()
        {
            //STARTED
            this.RadicalWindow.OptimizationStarted();

            // Run optimization with only the activeVariables
            double[] x = Design.ActiveVariables.Select(t => t.CurrentValue).ToArray();
            double[] query = x;
            double startingObjective = Design.Objectives[0];
            NloptResult result = Solver.Optimize(x, out MinValue);

            //FINISHED
            //Updates graph if the optimization ends in silent mode
            if (this.RadicalWindow.RadicalVM.Mode == RefreshMode.Silent)
            {
                AppendStoredValues();
            }
            this.RadicalWindow.OptimizationFinished();

            return result;
        }

        //Adds optimization values to array that updates graph display 
        public void AppendStoredValues()
        {
            Design.ScoreEvolution.AddRange(StoredMainValues);
            StoredMainValues.Clear();

            for (int i = 0; i < Design.ConstraintEvolution.Count; i++)
            {
                Design.ConstraintEvolution[i].AddRange(StoredConstraintValues[i]);
                StoredConstraintValues[i].Clear();
            }
        }

        public double constraint(double[] x, Constraint c)
        {
            for (int i = 0; i < nVars; i++)
            {
                IVariable var = Design.Variables[i];
                var.UpdateValue(x[i]);
            }
            foreach (IDesignGeometry vargeo in Design.Geometries)
            {
                vargeo.Update();
            }
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true, Grasshopper.Kernel.GH_SolutionMode.Silent);
            return c.CurrentValue - c.LimitValue;
        }

        //UNIMPLEMENTED
        public double Objective(double[] x, ref double[] grad)
        {
            for (int i = 0; i < nVars; i++)
            {
                IVariable var = Design.Variables[i];
                var.UpdateValue(x[i]);
            }

            if (grad != null) { }//update gradient, for gradient-based algs.

            double objective = Design.Objectives[0];
            Design.ScoreEvolution.Add(objective);
            return objective;
        }
    }
}
