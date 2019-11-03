using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Google.OrTools.Sat;
using Optimization.Models.Application;
using Optimization.Models.Architecture;

namespace Optimization
{
    internal static class Program
    {
        private const string CaseNumber = "1";
        private static readonly string DirectoryName = $"..\\..\\..\\TestCases\\Case {CaseNumber}\\";
        private static Application _application;
        private static Architecture _architecture;

        private static void Main()
        {
            // -- CREATE THE MODEL --
            var model = new CpModel();
            
            // -- SET UP DATA --
            ParseDocuments();

            var tasks = _application.Tasks;
            var cpus = _architecture.Cpus;
            
            var taskCount = tasks.Count;
            var cpuCount = cpus.Count;
            
            var intervals = cpus.Select(cpu => new List<IntervalVar>()).ToList();

            // -- ADD VARIABLES --
            for (var cpu = 0; cpu < cpuCount; cpu++)
            {
                for (var core = 0; core < cpus[cpu].Cores.Count; core++)
                {
                    for (var task = 0; task < taskCount; task++)
                    {
                        AssignVariables(cpu, core, tasks[task], model, intervals);
                    }
                }
            }

            // -- ADD CONSTRAINTS --
            // Ensure nothing is scheduled concurrently on the same core
            for (var cpu = 0; cpu < cpuCount; cpu++)
            {
                model.AddNoOverlap(intervals[cpu]);
            }
            
            // Ensure tasks are scheduled in the correct order according to the chains
            
            // Ensure tasks are scheduled on their specified core and cpu
            
            
            // -- ADD OBJECTIVE --


            // -- SOLVE --
            var solver = new CpSolver();
            solver.Solve(model);

            // -- PRINT SOLUTION --
        }

        private static void ParseDocuments()
        {
            var applicationDir = Directory.GetFiles(DirectoryName, "*.tsk")[0];
            var architectureDir = Directory.GetFiles(DirectoryName, "*.cfg")[0];

            var applicationDoc = new XmlDocument();
            var architectureDoc = new XmlDocument();

            architectureDoc.Load(architectureDir);
            applicationDoc.Load(applicationDir);

            _architecture = XmlParser.ParseArchitecture(architectureDoc);
            _application = XmlParser.ParseApplication(applicationDoc);
        }

        private static void AssignVariables(int cpu, int core, Task taskNode, CpModel model, List<List<IntervalVar>> intervals)
        {
            var suffix = $"_{cpu}_{core}_{taskNode.Name}";
            
            var start = model.NewIntVar(taskNode.Offset, taskNode.Deadline, $"start{suffix}");
            var end = model.NewIntVar(taskNode.Offset, taskNode.Deadline, $"end{suffix}");
            var active = model.NewBoolVar($"{taskNode.Name}");
            
            var interval = model.NewOptionalIntervalVar(start, taskNode.Wcet, end, active, $"interval{suffix}");

            intervals[cpu].Add(interval);
        }
    }
}