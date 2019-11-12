using Google.OrTools.Sat;

namespace OptimizationExercise.Models
{
    public class Task
    {
        public IntVar Start { get; set; }
        public IntVar End { get; set; }
        public IntervalVar Interval { get; set; }
        public IntVar IsActive { get; set; }
        public string Id { get; set; }
    }
}