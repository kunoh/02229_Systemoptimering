using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Google.OrTools.Sat;
using Optimization.Models;
using Optimization.Models.Application;
using Optimization.Models.Architecture;

namespace Optimization
{
    internal static class Program
    {
        private const string CaseNumber = "2";
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
            var periods = new Dictionary<(int core, string Id), List<TaskVar>>();

            // Initialize list of intervals on a per cpu per core basis
            for (var cpu = 0; cpu < cpuCount; cpu++) 
            {
                intervals.Add(new List<List<IntervalVar>>());
                for (var core = 0; core < cpus[cpu].Cores.Count; core++)
                {
                    intervals[cpu].Add(new List<IntervalVar>());
                }
            }

            var taskVars = new Dictionary<(int, int, int, int), TaskVar>();
            
            var chainTaskCount = new Dictionary<string, int>();

            for (var task = 0; task < taskCount; task++)
            {
                var taskNode = tasks[task];
                var tempCount = 0;

                for (var chain = 0; chain < _application.Chains.Count; chain++)
                {
                    tempCount = _application.Chains[chain].Runnables.Count(t => taskNode.Name.Equals(t));
                        
                    if (chainTaskCount.TryGetValue(taskNode.Id, out var value))
                    {
                        if (value < tempCount)
                        {
                            chainTaskCount[taskNode.Id] = tempCount;
                        }
                    }
                    else if(tempCount != 0)
                    {
                        chainTaskCount.Add(taskNode.Id, tempCount);
                    }
                }

                if (!chainTaskCount.TryGetValue(taskNode.Id, out var t))
                {
                    chainTaskCount.Add(taskNode.Id, 1);
                }
            }

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
                        if (cpu != taskNode.CpuId) continue;
                        
                        // If current node is not assigned to current core, and is not able to run on any core, skip
                        if (core != taskNode.CoreId && taskNode.CoreId != -1) continue;
                        
                        // If current task does not appear in any chain, skip it.
                        if (!chainTaskCount.TryGetValue(taskNode.Id, out var amount)) continue;
                        
                        periods[(core, taskNode.Id)] = new List<TaskVar>();
                        for (var count = 0; count < amount; count++)
                        {
                            var taskVar = AssignVariables(cpu, core, taskNode, model, count);
                            periods[(core, taskNode.Id)].Add(taskVar);
                            intervals[cpu][core].Add(taskVar.Interval);
                            taskVars[(cpu, core, task, count)] = taskVar;
                        }
                    }
                }
            }

            // Add a period as an interval, to ensure a task is not planned until its period has passed
//            foreach (var (key, value) in periods)
//            {
//                var count = value.Count;
//                for (var interval = 0; interval < count - 1; interval++)
//                {
//                    value.Add(new TaskVar
//                    {
//                        Interval = model.NewIntervalVar(
//                            value[interval].End,
//                            tasks.FirstOrDefault(t => t.Id == key.Id).Period,
//                            value[interval + 1].Start, "")
//                    });
//                }
//            }

            // -- ADD CONSTRAINTS --
            // Ensure nothing is scheduled concurrently on the same core
            for (var cpu = 0; cpu < cpuCount; cpu++)
            {
                for (var core = 0; core < cpus[cpu].Cores.Count; core++)
                {
                    model.AddNoOverlap(intervals[cpu][core]);
                }
            }

            foreach (var ((_, id), value) in periods)
            {
                for (var task = 0; task < value.Count - 1; task++)
                {
                    var taskIntervals = periods.Where(tI => tI.Key.Id == id).Select(tI => tI.Value).ToList();

                    foreach (var taskInterval in taskIntervals)
                    {
                        model.Add(taskInterval[task + 1].Start - value[task].End >= tasks.FirstOrDefault(t => t.Id == id).Period);
                    }
                }
            }

            // If a task can run on any core, ensure it is only scheduled for one core
            for (var task = 0; task < taskCount; task++)
            {
                var currentTask = tasks[task];
                if (chainTaskCount.TryGetValue(currentTask.Id, out var uniqueTasks))
                {
                    for (var count = 0; count < uniqueTasks; count++)
                    {
                        var assigned = new List<IntVar>();
                        for (var core = 0; core < cpus[currentTask.CpuId].Cores.Count; core++)
                        {
                            if (taskVars.TryGetValue((currentTask.CpuId, core, task, count), out var assignTask))
                            {
                                assigned.Add(assignTask.IsActive);
                            }
                        }
                        model.Add(LinearExpr.Sum(assigned) == 1);
                    }
                }
            }

            // -- ADD OBJECTIVE --
            var makespan = model.NewIntVar(0, 9999999999, "makespan");
            var endTimes = taskVars.Select(task => task.Value.End).ToList();
            model.AddMaxEquality(makespan, endTimes);
            model.Minimize(makespan);

            // -- SOLVE --
            var solver = new CpSolver();
            var status = solver.Solve(model);
            solver.SearchAllSolutions(model, new SolutionPrinter(taskVars, cpus));
            
            if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
            {
                Console.WriteLine("Solution found!!!");
                SaveSolution(taskVars, solver);
            }
        }

        /// <summary>
        /// Saves a solution as an xml file
        /// </summary>
        /// <param name="taskVars"></param>
        /// <param name="solver"></param>
        /// <param name="scenario">Contains all information about the current problem</param>
        /// <param name="assignedTasks">List containing all assigned tasks sorted by start time</param>
        private static void SaveSolution(Dictionary<(int, int, int, int), TaskVar> taskVars, CpSolver solver)
        {
            var assignedTasks = new List<AssignedTask>();

            foreach (var ((cpu, core, _, _), task) in taskVars)
            {
                if (solver.Value(task.IsActive) == 1)
                {
                    assignedTasks.Add(new AssignedTask
                    {
                        Cpu = cpu.ToString(),
                        Core = core.ToString(),
                        Start = solver.Value(task.Start),
                        End = solver.Value(task.End),
                        Suffix = task.Suffix
                    });
                }
            }
            var solution = new XmlDocument();
            var version = solution.CreateXmlDeclaration("1.0", null, null);
            solution.AppendChild(version);

            var tables = solution.CreateElement("Tables");
            solution.AppendChild(tables);

            foreach (var assignedTask in assignedTasks)
            {
                Console.Write(assignedTask.Cpu);
            }

            solution.Save($"Case{CaseNumber}.xml");
        }

        private static TaskVar AssignVariables(int cpu, int core, Task taskNode, CpModel model, int count)
        {
            var suffix = $"_{count}_{cpu}_{core}_{taskNode.Name}";

            var start = model.NewIntVar(taskNode.Offset, taskNode.Deadline + (count * taskNode.Period), $"start{suffix}");
            var end = model.NewIntVar(taskNode.Offset, taskNode.Deadline + (count * taskNode.Period), $"end{suffix}");
            var isActive = model.NewBoolVar($"{taskNode.Name}");

            var interval = model.NewOptionalIntervalVar(start, taskNode.Wcet, end, isActive, $"interval{suffix}");

            return new TaskVar {
                Start = start,
                End = end,
                Interval = interval,
                IsActive = isActive,
                Suffix = suffix
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