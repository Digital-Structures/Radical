﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLoptNet;
using System.Threading;
using System.Threading.Tasks;
using Radical.TestComponents;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;

namespace Radical.Integration
{
    public class Optimizer

    {
        //static bool verbose = true;
        public double? MinValue; //init objective

        // CONSTRUCTOR FOR RADICAL
        public Optimizer(IDesign design, RadicalWindow radicalWindow)
        {
            this.Design = design;
            this.MainAlg = radicalWindow.RadicalVM.PrimaryAlgorithm;
            //this.SecondaryAlg = NLoptAlgorithm.LN_COBYLA;
            this.RadicalWindow = radicalWindow;
            BuildWrapper();
            SetBounds();
            Solver.SetMinObjective((x) => Objective(x));
            if (Design.Constraints != null)
            {
                foreach (Constraint c in Design.Constraints)
                {
                    if (c.IsActive)
                    {
                        if (c.ConstraintType == ConstraintType.lessthan)
                            Solver.AddLessOrEqualZeroConstraint((x) => Constraint(x, c));
                        else if (c.ConstraintType == ConstraintType.morethan)
                            Solver.AddLessOrEqualZeroConstraint((x) => -Constraint(x, c));
                        else
                            Solver.AddEqualZeroConstraint((x) => Constraint(x, c));
                    }
                }
            }
        }

        public IDesign Design;
        public uint nVars { get { return (uint)this.Design.ActiveVariables.Count; } }

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

            //Adds main objective values to list and draws
            double objective = Design.CurrentScore;
            Design.ScoreEvolution.Add(objective);

            ((Design)Design).OptComponent.Evolution = Design.ScoreEvolution.ToList();

            for (int i = 0; i < Design.ConstraintEvolution.Count; i++)
            {
                double score = Design.Constraints[i].CurrentValue;
                Design.ConstraintEvolution[i].Add(score);
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

        public double Constraint(double[] x, Constraint c)
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

        public NloptResult RunOptimization()
        {
            //STARTED
            this.RadicalWindow.OptimizationStarted();

            // Run optimization with only the activeVariables
            double[] x = Design.ActiveVariables.Select(t => t.CurrentValue).ToArray();
            double[] query = x;
            double startingObjective = Design.CurrentScore;
            NloptResult result = Solver.Optimize(x, out MinValue);

            //FINISHED
            this.RadicalWindow.OptimizationFinished();

            return result;
        }

        #region obsolete_constructors
        public Optimizer(IDesign design)
        {
            Design = design;
            this.MainAlg = NLoptAlgorithm.LD_LBFGS;
            BuildWrapper();
            SetBounds();
            Solver.SetMinObjective((x) => Objective(x));
            if (Design.Constraints != null)
            {
                foreach (Constraint c in Design.Constraints)
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
                    Solver.AddLessOrEqualZeroConstraint((x) => Constraint(x, Design.Constraints[i]));
                }
            }
        }
        #endregion
    }
}
