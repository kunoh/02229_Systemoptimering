﻿using System;
using System.Collections.Generic;
using System.Text;
using Google.OrTools.Sat;

namespace Optimization
{
    class TaskVar
    {
        public IntVar Start { get; set; }
        public IntVar End { get; set; }
        public IntervalVar Interval { get; set; }
        public IntVar IsActive { get; set; }
        public string Suffix { get; set; }
        
        public string Id { get; set; }
        
        public int Duration { get; set; }
    }
}
