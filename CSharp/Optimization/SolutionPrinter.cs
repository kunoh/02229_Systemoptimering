using Google.OrTools.Sat;
using Optimization.Models;
using Optimization.Models.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optimization
{
    class SolutionPrinter : CpSolverSolutionCallback
    {
        Dictionary<(int, int, int, int), TaskVar> taskVarDict;
        List<Cpu> cpus;

        public SolutionPrinter(Dictionary<(int, int, int, int), TaskVar> taskVarDict, List<Cpu> cpus)
        {
            this.taskVarDict = taskVarDict;
            this.cpus = cpus;
        }

        public override void OnSolutionCallback()
        {
            string output = "";
            var assignedTasks = new List<AssignedTask>();

            foreach (var ((cpu, core, _, _), task) in taskVarDict)
            {
                if (Value(task.IsActive) == 1)
                {
                    assignedTasks.Add(new AssignedTask
                    {
                        Cpu = cpu.ToString(),
                        Core = core.ToString(),
                        Start = Value(task.Start),
                        End = Value(task.End),
                        Suffix = task.Suffix

                    });
                }
            }
            
            foreach (var cpu in cpus)
            {
                output += $"Cpu {cpu.Id}: \n";
                foreach (var core in cpu.Cores)
                {
                    output += $"Core {core.Id}: ";

                    foreach (var task in assignedTasks.Where(t => t.Cpu.Equals(cpu.Id) && t.Core.Equals(core.Id)).OrderBy(s => s.Start))
                    {
                        output += $"{task.Suffix}[{task.Start} - {task.End}] ";
                    }

                    output += "\n";
                }

                output += "\n";
            }

            Console.WriteLine(output);
        }
        

    }
}
