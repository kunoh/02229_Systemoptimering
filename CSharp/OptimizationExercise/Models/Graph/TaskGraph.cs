using System.Collections.Generic;

namespace OptimizationExercise.Models.Graph
{
    public class TaskGraph
    {
        public string Name { get; set; }
        public List<Edge> Edges { get; set; }
    }
}