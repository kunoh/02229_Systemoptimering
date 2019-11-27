using Google.OrTools.Sat;
using Optimization.Models;
using Optimization.Models.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            foreach (var task in taskVarDict)
            {
                if (Value(task.Value.IsActive) == 1)
                {
                    assignedTasks.Add(new AssignedTask
                    {
                        Cpu = task.Key.Item1.ToString(),
                        Core = task.Key.Item2.ToString(),
                        Index = task.Key.Item3,
                        Count = task.Key.Item4,
                        Start = Value(task.Value.Start),
                        End = Value(task.Value.End)
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
                        output += $"{task.Index}{task.Count}[{task.Start} - {task.End}] ";
                    }

                    output += "\n";
                }

                output += "\n";
            }

            Console.WriteLine(output);
        }
        

    }
}
