using System.Collections.Generic;

namespace Optimization.Models.Architecture
{
    /// <summary>
    /// The architecture has at least one CPU and at least one Core for each CPU.
    /// </summary>
    public class Architecture
    {
        public List<Cpu> Cpus { get; set; }
    }
}