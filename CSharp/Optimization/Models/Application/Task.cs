using System;
using System.Xml;

namespace Optimization.Models.Application
{
    public class Task
    {
        public Task(XmlNode taskNode)
        {
            this._node = taskNode;
            this.Id = GetValue("Id");
            this.Name = GetValue("Name");
            this.Wcet = GetIntValue("WCET");
            this.Period = GetIntValue("Period");
            this.Deadline = GetIntValue("Deadline");
            this.MaxJitter = GetIntValue("MaxJitter");
            this.Offset = GetIntValue("Offset");
            this.CpuId = GetValue("CpuId");
            this.CoreId = GetValue("CpuId");
        }

        /// <summary>
        /// Unique ID
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        /// A unique name for the task
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Worst Case Execution Time of the task; in microseconds
        /// </summary>
        public int Wcet { get; }
        
        /// <summary>
        /// Period of a task in microseconds
        /// </summary>
        public int Period { get; }
        
        /// <summary>
        /// Deadline of a task in microseconds
        /// </summary>
        public int Deadline { get; }
        
        /// <summary>
        /// Maximum Jitter in microseconds that the task can have. -1 for no jitter limitations.
        /// </summary>
        public int MaxJitter { get; }
        
        /// <summary>
        /// The earliest activation time of task within its period in microseconds.
        /// </summary>
        public int Offset { get; }
        
        /// <summary>
        /// Id of the assigned CPU. Must be assigned.
        /// </summary>
        public string CpuId { get; }
        
        /// <summary>
        /// Id of the assigned Core of the CPU; optional; -1 for no core assignment.
        /// </summary>
        public string CoreId { get; }

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