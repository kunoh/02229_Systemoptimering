using System;
using System.Collections.Generic;
using System.Xml;

namespace Optimization.Models.Application
{
    /// <summary>
    /// A chain shows task flow
    /// </summary>
    public class Chain
    {
        /// <summary>
        /// End to End Response of the chain in microseconds.
        /// </summary>
        public int Budget { get; set; }

        /// <summary>
        /// [0,1]; shows the Priority of the chain. 1 is the highest Priority and 0 is the lowest.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// A unique name for the chain.
        /// </summary>
        public string Name { get; set; }

        public List<string> Runnables { get; set; }
    }
}