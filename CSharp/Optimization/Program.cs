using System;
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

            var intervals = new List<List<List<IntervalVar>>>();

            // Initialize list of intervals on a per cpu per core basis
            for (var cpu = 0; cpu < cpuCount; cpu++) 
            {
                intervals.Add(new List<List<IntervalVar>>());
                for (var core = 0; core < cpus[cpu].Cores.Count; core++)
                {
                    intervals[cpu].Add(new List<IntervalVar>());
                }
            }

            var taskVars = new Dictionary<(int, int, int), TaskVar>();

            // -- ADD VARIABLES --
            for (var cpu = 0; cpu < cpuCount; cpu++)
            {
                for (var core = 0; core < cpus[cpu].Cores.Count; core++)
                {
                    // TODO: Assign tasks to CPU, to avoid iterating all tasks for each cpu
                    for (var task = 0; task < taskCount; task++)
                    {
                        var taskNode = tasks[task];
                        
                        // If current node is not assigned to the current CPU skip it.
                        // If current node is not assigned to current core, and is not able to run on any core, skip
                        if (cpu != taskNode.CpuId && core != taskNode.CoreId && taskNode.CoreId != -1) continue;
                        
                        var taskVar = AssignVariables(cpu, core, taskNode, model);

                        intervals[cpu][core].Add(taskVar.Interval);
                        taskVars[(cpu, core, task)] = taskVar;
                    }
                }
            }

            // -- ADD CONSTRAINTS --
            // Ensure nothing is scheduled concurrently on the same core
            for (var cpu = 0; cpu < cpuCount; cpu++)
            {
                for (var core = 0; core < cpus[cpu].Cores.Count; core++)
                {
                    model.AddNoOverlap(intervals[cpu][core]);
                }
            }

            // If a task can run on any core, ensure it is only scheduled for one core
            for (var task = 0; task < taskCount; task++)
            {
                var currentTask = tasks[task];
                
                // Skip tasks if it has a specific core assigned
                if (currentTask.CoreId != -1) continue;
                
                var assigned = new List<IntVar>();
                for (var core = 0; core < cpus[currentTask.CpuId].Cores.Count; core++)
                {
                    assigned.Add(taskVars[(currentTask.CpuId, core, task)].IsActive);
                }
                model.Add(LinearExpr.Sum(assigned) == 1);
            }

            // -- ADD OBJECTIVE --
            var makespan = model.NewIntVar(0, 80000, "makespan");
            var endTimes = taskVars.Select(task => task.Value.End).ToList();
            model.AddMaxEquality(makespan, endTimes);
            model.Minimize(makespan);

            // -- SOLVE --
            var solver = new CpSolver();
            var status = solver.Solve(model);

            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Console.WriteLine("Solution found!!!"); 
            }

            // -- PRINT SOLUTION --
        }

        private static TaskVar AssignVariables(int cpu, int core, Task taskNode, CpModel model)
        {
            var suffix = $"_{cpu}_{core}_{taskNode.Name}";

            var start = model.NewIntVar(taskNode.Offset, taskNode.Deadline, $"start{suffix}");
            var end = model.NewIntVar(taskNode.Offset, taskNode.Deadline, $"end{suffix}");
            var isActive = model.NewBoolVar($"{taskNode.Name}");

            var interval = model.NewOptionalIntervalVar(start, taskNode.Wcet, end, isActive, $"interval{suffix}");

            return new TaskVar {
                Start = start,
                End = end,
                Interval = interval,
                IsActive = isActive
            };
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
    }
}