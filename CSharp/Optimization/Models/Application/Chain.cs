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
        public Chain(XmlNode node)
        {
            _node = node;

            this.Budget = GetIntValue("Budget");
            this.Priority = GetIntValue("Priority");
            this.Name = GetValue("Name");

            this.Runnables = new List<string>();
            
            foreach (XmlNode runnableNode in node.ChildNodes)
            {
                this.Runnables.Add(runnableNode.Attributes.GetNamedItem("Name").Value);
            }
        }
        
        /// <summary>
        /// End to End Response of the chain in microseconds.
        /// </summary>
        public int Budget { get; }
        
        /// <summary>
        /// [0,1]; shows the Priority of the chain. 1 is the highest Priority and 0 is the lowest.
        /// </summary>
        public int Priority { get; }
        
        /// <summary>
        /// A unique name for the chain.
        /// </summary>
        public string Name { get; }
        
        public List<string> Runnables { get; }

        private readonly XmlNode _node;

        private string GetValue(string attr)
        {
            return _node.Attributes.GetNamedItem(attr).Value;
        }

        private int GetIntValue(string attr)
        {
            return Convert.ToInt32(_node.Attributes.GetNamedItem(attr).Value);
        }
    }
}