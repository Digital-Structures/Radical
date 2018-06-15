using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Radical.Components;
using Radical.TestComponents;


namespace Radical.Integration
{
    public class Design : IDesign
    {
        //Radical Component
        public Design(List<IVariable> vars, RadicalComponent component)
        {
            this.Variables = vars;
            this.ScoreEvolution = new List<double>();
            this.OptComponent = component;
        }

        public Design(List<IVariable> vars, List<IConstraint> consts, RadicalComponent component)
        {
            this.Variables = vars;
            this.Constraints = consts;
            this.ScoreEvolution = new List<double>();
            this.OptComponent = component;
        }

        public Design(List<IVariable> vars, List<IDesignGeometry> geos, List<IConstraint> consts, RadicalComponent component)
        {
            this.Variables = vars;
            this.Constraints = consts;
            this.Geometries = geos;
            if (Geometries.Any()) { this.Variables.AddRange(Geometries.Select(x => x.Variables).SelectMany(x => x).ToList()); } // not the cleanest way to do it, review code structure
            this.ScoreEvolution = new List<double>();
            this.OptComponent = component;
        }

        public double CurrentScore
        {
            get
            {
                return OptComponent.Objective;
            }
            set { }
        }

        public List<double> ConstraintsNumber
        {
            get { return OptComponent.Constraints; }
        }
        public List<List<double>> Samples { get; set; }
        public List<List<double>> Properties { get; set; }

        public IOptimizationComponent OptComponent { get; set; }

        public IExplorationComponent ExpComponent;

        public List<double> ScoreEvolution { get; set; }

        public IGH_Param ScoreParameter { get; set; }

        public List<IVariable> Variables { get; set; }

        public List<IDesignGeometry> Geometries { get; set; }

        public List<IConstraint> Constraints { get; set; }

        public void Optimize()
        {
            Optimizer opt = new Optimizer(this);
            opt.RunOptimization();
            this.OptComponent.Evolution = this.ScoreEvolution;
            Grasshopper.Instances.ActiveCanvas.Document.NewSolution(true);
        }

        public void Optimize(RadicalWindow radicalWindow)
        {
            Optimizer opt = new Optimizer(this, radicalWindow);
            opt.RunOptimization();
            this.OptComponent.Evolution = this.ScoreEvolution;
        }

        public void Sample(int alg)
        {

            Sampler.ISamplingAlg samplingAlg;
            switch (alg)
            {
                case 0:
                    samplingAlg = new Sampler.GSampling();
                    break;
                case 1:
                    samplingAlg = new Sampler.RUSampling();
                    break;
                case 2:
                    samplingAlg = new Sampler.LHSampling();
                    break;
                default:
                    samplingAlg = new Sampler.LHSampling();
                    break;
            }
            Sampler sam = new Sampler(this, samplingAlg, ExpComponent.nSamples);
            sam.RunSampling();
        }

        #region obsolete_constructors
        public Design(List<IVariable> vars, IExplorationComponent component)
        {
            this.Variables = vars;
            //this.CurrentScore = 0; //init objective function
            this.Samples = new List<List<double>>();
            this.Properties = new List<List<double>>();
            this.ExpComponent = component;
        }

        public Design(List<IVariable> vars, IOptimizationComponent component)
        {
            this.Variables = vars;
            //this.CurrentScore = 0; //init objective function
            this.ScoreEvolution = new List<double>();
            this.OptComponent = component;
        }

        public Design(List<IVariable> vars, List<IDesignGeometry> geos, IOptimizationComponent component)
        {
            this.Variables = vars;
            this.Geometries = geos;
            if (Geometries.Any()) { this.Variables.AddRange(Geometries.Select(x => x.Variables).SelectMany(x => x).ToList()); } // not the cleanest way to do it, review code structure
            //this.CurrentScore = 0; //init objective function
            this.ScoreEvolution = new List<double>();
            this.OptComponent = component;
        }
        #endregion
    }
}
