using System.Collections.Generic;
using System.Linq;
using Google.OrTools.Sat;
using Optimization.Models.Application;
using Optimization.Models.Architecture;
using OptimizationExercise;

namespace Optimization
{
    internal static class Program
    {
        private const string CaseNumber = "1";
        private static readonly string DirectoryName = $"..\\..\\..\\TestCases\\Case {CaseNumber}\\";
        private static readonly string ApplicationFileName = $"{DirectoryName}Case{CaseNumber}.tsk";
        private static readonly string ArchitectureFileName = $"{DirectoryName}Case{CaseNumber}.cfg";

        private static void Main(string[] args)
        {
            // -- CREATE THE MODEL --
            var model = new CpModel();
            
            // -- SET UP THE DATA --
            var architecture = new Architecture(new XmlParser(DirectoryName, "cfg").Document);
            var application = new Application(new XmlParser(DirectoryName, "tsk").Document);

            var tasks = application.Tasks;
            var cpus = architecture.Cpus;
            var taskCount = tasks.Count;
            var cpuCount = cpus.Count;
            var intervals = cpus.Select(cpu => new List<IntervalVar>()).ToList();

            for (var cpu = 0; cpu < cpuCount; cpu++)
            {
                for (var core = 0; core < cpus[cpu].Cores.Count; core++)
                {
                    for (var task = 0; task < taskCount; task++)
                    {
                        var taskNode = tasks[task];
                        var suffix = $"_{cpu}_{core}_{taskNode.Name}";
                        var start = model.NewIntVar(taskNode.Offset, taskNode.Deadline, $"start{suffix}");
                        var end = model.NewIntVar(taskNode.Offset, taskNode.Deadline, $"end{suffix}");
                        var active = model.NewBoolVar($"{taskNode.Name}");
                        var interval = model.NewOptionalIntervalVar(start, taskNode.Wcet, end, active, $"interval{suffix}");
                        
                        intervals[cpu].Add(interval);
                    }
                }
            }

            for (var cpu = 0; cpu < cpuCount; cpu++)
            {
                model.AddNoOverlap(intervals[cpu]);
            }
        }
    }
}