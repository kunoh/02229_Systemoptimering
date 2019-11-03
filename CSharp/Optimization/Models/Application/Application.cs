using System.Collections.Generic;

namespace Optimization.Models.Application
{
    /// <summary>
    /// The application has tasks and chains. A task is tagged with "Node"
    /// </summary>
    public class Application
    {
        public List<Task> Tasks { get; set; }
        public List<Chain> Chains { get; set; }
    }
}
