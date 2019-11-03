using System.Collections.Generic;
using System.Xml;

namespace Optimization.Models.Architecture
{
    /// <summary>
    /// The CPU element, attributed with an "Id" tag which is a unique integer.
    /// It has children tagged with "Core"
    /// </summary>
    public class Cpu
    {
        public string Id { get; set; }
        public List<Core> Cores { get; set; }
    }
}