using System;
using System.Xml;

namespace Optimization.Models.Application
{
    public class Task
    {
        /// <summary>
        /// Unique ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// A unique name for the task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Worst Case Execution Time of the task; in microseconds
        /// </summary>
        public int Wcet { get; set; }

        /// <summary>
        /// Period of a task in microseconds
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Deadline of a task in microseconds
        /// </summary>
        public int Deadline { get; set; }

        /// <summary>
        /// Maximum Jitter in microseconds that the task can have. -1 for no jitter limitations.
        /// </summary>
        public int MaxJitter { get; set; }

        /// <summary>
        /// The earliest activation time of task within its period in microseconds.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Id of the assigned CPU. Must be assigned.
        /// </summary>
        public string CpuId { get; set; }

        /// <summary>
        /// Id of the assigned Core of the CPU; optional; -1 for no core assignment.
        /// </summary>
        public string CoreId { get; set; }
    }
}