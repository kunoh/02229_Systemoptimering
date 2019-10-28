namespace OptimizationExercise.Models.Graph
{
    public class Node
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Wcet { get; set; }
        public int Period { get; set; }
        public int Deadline { get; set; }
    }
}