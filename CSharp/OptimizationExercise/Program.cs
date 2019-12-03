using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Google.OrTools.Sat;
using OptimizationExercise.Models;
using OptimizationExercise.Models.Configuration;
using OptimizationExercise.Models.Graph;

namespace OptimizationExercise
{
    public static class Program
    {
        private const string CaseNumber = "2";
        private static readonly string DirectoryName = $"..\\..\\..\\Cases\\Case {CaseNumber}\\";
        private static readonly string GraphFileName = $"{DirectoryName}Case{CaseNumber}.tsk";
        private static readonly string CpuFileName = $"{DirectoryName}Case{CaseNumber}.cfg";
        
        private static int _horizon, _coreCount, _nodeCount;

        public static void Main()
        {
            // -- CREATE THE MODEL --
            var model = new CpModel();
            
            // -- SET UP THE DATA --
            var parsedCpu = new XmlParser(CpuFileName).Document;
            var parsedGraph = new XmlParser(GraphFileName).Document;
            
            var scenario = new Scenario
            {
                Cpus = InitializeCpus(parsedCpu),
                Graph = new Graph(parsedGraph)
            };

            _coreCount = scenario.Cpus.Sum(cpu => cpu.Cores.Count);
            _nodeCount = scenario.Graph.Nodes.Count;
            
            // Time to complete all tasks sequentially (Longest schedule)
            _horizon = scenario.Graph.Nodes.Sum(node => node.Wcet);

            // Initialize list to store intervals
            var coreIntervals = (
                from cores in scenario.Cpus.Select(cpu => cpu.Cores) 
                from core in cores 
                select new List<IntervalVar>())
                .ToList();
            
            // Map of tasks with tuple of core and node id
            var tasks = new Dictionary<(int, int), Task>();

            // Loop to create the variables
            for (var core = 0; core < _coreCount; core++)
            {
                for (var node = 0; node < _nodeCount; node++)
                {
                    var nodeObject = scenario.Graph.Nodes[node];
                    var task = CreateVariables(core, model, nodeObject);
                    
                    // Keep track of variables for each task
                    tasks[(core, node)] = task;
                    
                    // Keep track of each interval on a per core basis
                    coreIntervals[core].Add(task.Interval);
                }
            }

            // Ensure nodes don't overlap
            for (var core = 0; core < _coreCount; core++)
            {
                model.AddNoOverlap(coreIntervals[core]);
            }

            // Ensure a task is only planned once
            for (var node = 0; node < _nodeCount; node++)
            {
                var assigned = new List<IntVar>();
                for (var core = 0; core < _coreCount; core++)
                {
                    assigned.Add(tasks[(core, node)].IsActive);
                }

                model.Add(LinearExpr.Sum(assigned) == 1);
            }

            // Ensure that a node isn't scheduled until after its predecessor has finished.
            // Iterates over each task graph and ensures that for each node
            // any destinations are only planned for once the current node has finished.
            foreach (var edge in scenario.Graph.TaskGraphs.SelectMany(taskGraph => taskGraph.Edges))
            {
                for (var core = 0; core < _coreCount; core++)
                {
                    for (var node = 0; node < _nodeCount; node++)
                    {
                        if (scenario.Graph.Nodes[node].Name != edge.Source) continue;
                        
                        foreach (var (c, n) in tasks)
                        {
                            if (scenario.Graph.Nodes[c.Item2].Name == edge.Destination)
                            {
                                model.Add(n.Start >= tasks[(core, node)].End);
                            }
                        }
                    }
                }
            }

            var makespan = model.NewIntVar(0, _horizon, "makespan");
            var endTimes = tasks.Select(task => task.Value.End).ToList();
            model.AddMaxEquality(makespan, endTimes);
            model.Minimize(makespan);

            var solver = new CpSolver(); 
            solver.SearchAllSolutions(model, new Printer(tasks, _coreCount, _nodeCount, scenario));
            var status = solver.Solve(model);

            if (status != CpSolverStatus.Optimal) return;
            {
                var output = string.Empty;
                
                var assignedTasks = new List<List<AssignedTask>>();

                for (var core = 0; core < _coreCount; core++)
                {
                    assignedTasks.Add(new List<AssignedTask>());
                }

                for (var core = 0; core < _coreCount; core++)
                {
                    for (var node = 0; node < _nodeCount; node++)
                    {
                        assignedTasks[core].Add(new AssignedTask
                        {
                            Start = solver.Value(tasks[(core, node)].Start),
                            Core = core,
                            Index = scenario.Graph.Nodes[node].Name,
                            Duration = scenario.Graph.Nodes[node].Wcet,
                            IsActive = solver.Value(tasks[(core, node)].IsActive),
                            Id = tasks[(core, node)].Id
                        });
                    }
                }

                for (var core = 0; core < _coreCount; core++)
                {
                    assignedTasks[core] = assignedTasks[core].OrderBy(task => task.Start).ToList();
                }

                Console.WriteLine($"Optimal Schedule Length: {solver.ObjectiveValue}");
                Console.Write(output);
                
                SaveSolution(scenario, assignedTasks);
            }
        }

        /// <summary>
        /// Creates a <see cref="Task"/> based on the given input
        /// </summary>
        /// <param name="core">Index of the core the variables are created for</param>
        /// <param name="model">The model to which the variables are added</param>
        /// <param name="nodeObject">Node object containing information about the node</param>
        /// <returns>A <see cref="Task"/> object</returns>
        private static Task CreateVariables(int core, CpModel model, Node nodeObject)
        {
            var start = model.NewIntVar(0, nodeObject.Deadline, $"start_{core}_{nodeObject.Name}");
            var end = model.NewIntVar(0, nodeObject.Deadline, $"end_{core}_{nodeObject.Name}");
            var active = model.NewBoolVar($"{nodeObject.Name}");
            var interval = model.NewOptionalIntervalVar(
                start, nodeObject.Wcet, end, active, $"interval_{core}_{nodeObject.Name}");

            return new Task
            {
                Start = start,
                End = end,
                Interval = interval,
                IsActive = active,
                Id = nodeObject.Id
            };
        }

        /// <summary>
        /// Saves a solution as an xml file
        /// </summary>
        /// <param name="scenario">Contains all information about the current problem</param>
        /// <param name="assignedTasks">List containing all assigned tasks sorted by start time</param>
        private static void SaveSolution(Scenario scenario, List<List<AssignedTask>> assignedTasks)
        {
            var solution = new XmlDocument();
            var version = solution.CreateXmlDeclaration("1.0", null, null);
            solution.AppendChild(version);

            var tables = solution.CreateElement("Tables");
            solution.AppendChild(tables);

            foreach (var cpu in scenario.Cpus)
            {
                for (var core = 0; core < cpu.Cores.Count; core++)
                {
                    var cpuId = solution.CreateAttribute("CpuId");
                    cpuId.Value = cpu.Id;

                    var schedule = solution.CreateElement("Schedule");
                    var coreId = solution.CreateAttribute("CoreId");

                    coreId.Value = cpu.Cores[core].Id;
                    schedule.Attributes.Append(coreId);
                    schedule.Attributes.Append(cpuId);

                    tables.AppendChild(schedule);

                    for (var node = 0; node < _nodeCount; node++)
                    {
                        var task = assignedTasks[core][node];

                        if (task.IsActive != 1) continue;

                        var slice = solution.CreateElement("Slice");
                        var start = solution.CreateAttribute("Start");
                        var duration = solution.CreateAttribute("Duration");
                        var taskId = solution.CreateAttribute("TaskId");

                        start.Value = task.Start.ToString();
                        duration.Value = task.Duration.ToString();
                        taskId.Value = task.Id;
                        slice.Attributes.Append(start);
                        slice.Attributes.Append(duration);
                        slice.Attributes.Append(taskId);

                        schedule.AppendChild(slice);
                    }
                }
            }

            solution.Save($"Case{CaseNumber}.xml");
        }

        private static List<Cpu> InitializeCpus(XmlDocument parsedCpu)
        {
            return parsedCpu.GetElementsByTagName("Cpu").Cast<XmlNode>()
                .Select(cpu => new Cpu(cpu)).ToList();
        }
    }
}