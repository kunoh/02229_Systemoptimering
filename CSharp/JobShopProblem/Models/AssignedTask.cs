namespace JobShopProblem.Models
{
    public class AssignedTask
    {
        public long Start { get; set; }
        public int Job { get; set; }
        public int Index { get; set; }
        public int Duration { get; set; }
    }
}