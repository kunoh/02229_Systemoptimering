using System.Collections.Generic;
using OptimizationExercise.Models.Configuration;

namespace OptimizationExercise.Models
{
    public class Scenario
    {
        public List<Cpu> Cpus { get; set; }

        public Graph.Graph Graph { get; set; }
    }
}