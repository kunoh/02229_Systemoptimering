using System;
using System.Collections.Generic;
using System.Text;

namespace Optimization.Models
{
    class AssignedTask
    {
        public long Start { get; set; }
        public long End { get; set; }
        public string Cpu { get; set; }
        public string Core { get; set; }
        public int Index { get; set; }
        public int Count { get; set; }
    }
}
