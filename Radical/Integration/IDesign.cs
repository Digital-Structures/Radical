using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grasshopper.Kernel;
using Radical.Components;

namespace Radical.Integration
{
    public interface IDesign
    {
        double CurrentScore { get; set; }
        List<IVariable> Variables { get; set; }
        List<IDesignGeometry> Geometries { get; set; }
        List<IConstraint> Constraints { get; }
        List<List<double>> Samples {get;set;}
        List<List<double>> Properties { get; set; }
        List<double> ScoreEvolution { get; set; }
        void Optimize();
        void Optimize(RadicalWindow radicalWindow);
        void Sample(int alg);
        IGH_Param ScoreParameter { get; set; }
        IOptimizationComponent OptComponent { get; set; }
    }
}
