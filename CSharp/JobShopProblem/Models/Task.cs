using Google.OrTools.Sat;

namespace JobShopProblem.Models
{
    public class Task
    {
        public IntVar Start { get; set; }
        public IntVar End { get; set; }
        public IntervalVar Interval { get; set; }
    }
}