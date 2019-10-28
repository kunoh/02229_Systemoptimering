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

        public static void Main(string[] args)
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

            var coreCount = scenario.Cpus.Sum(cpu => cpu.Cores.Count);
            var nodeCount = scenario.Graph.Nodes.Count;
            var horizon = scenario.Graph.Nodes.Sum(node => node.Wcet);

            var coreIntervals = (
                from cores in scenario.Cpus.Select(cpu => cpu.Cores) 
                from core in cores 
                select new List<IntervalVar>())
                .ToList();
            
            var tasks = new Dictionary<(int, int), Task>();

            for (var core = 0; core < coreCount; core++)
            {
                for (var node = 0; node < nodeCount; node++)
                {
                    var nodeObject = scenario.Graph.Nodes[node];
                    
                    var start = model.NewIntVar(0, horizon, $"start_{core}_{nodeObject.Name}");
                    var end = model.NewIntVar(0, horizon, $"end_{core}_{nodeObject.Name}");
                    var interval = model.NewIntervalVar(start, nodeObject.Wcet, end, $"interval_{core}_{nodeObject.Name}");
                    var id = model.NewBoolVar($"{node}");
                    
                    tasks[(core, node)] = new Task
                    {
                        Start = start,
                        End = end,
                        Interval = interval,
                        Id = id
                    };
                    
                    coreIntervals[core].Add(interval);
                }
            }

            // Ensure nodes don't overlap
            for (var core = 0; core < coreCount; core++)
            {
                model.AddNoOverlap(coreIntervals[core]);;
            }

            // Ensure that a node isn't scheduled until after its predecessor has finished
            foreach (var edge in scenario.Graph.TaskGraphs.SelectMany(taskGraph => taskGraph.Edges))
            {
                for (var core = 0; core < coreCount; core++)
                {
                    for (var node = 0; node < nodeCount; node++)
                    {
                        if (scenario.Graph.Nodes[node].Name == edge.Source)
                        {
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
            }

            var makespan = model.NewIntVar(0, horizon, "makespan");
            var endTimes = tasks.Select(task => task.Value.End).ToList();
            model.AddMaxEquality(makespan, endTimes);
            model.Minimize(makespan);

            var solver = new CpSolver();
            var status = solver.Solve(model);

            if (status == CpSolverStatus.Optimal)
            {
                var output = string.Empty;
                
                var assignedTasks = new List<List<AssignedTask>>();

                for (var core = 0; core < coreCount; core++)
                {
                    assignedTasks.Add(new List<AssignedTask>());
                }

                for (var core = 0; core < coreCount; core++)
                {
                    for (var node = 0; node < nodeCount; node++)
                    {
                        var machine = core;
                        assignedTasks[machine].Add(new AssignedTask
                        {
                            Start = solver.Value(tasks[(core, node)].Start),
                            Core = core,
                            Index = scenario.Graph.Nodes[node].Name,
                            Duration = scenario.Graph.Nodes[node].Wcet
                        });
                    }
                }

                for (var core = 0; core < coreCount; core++)
                {
                    assignedTasks[core] = assignedTasks[core].OrderBy(task => task.Start).ToList();
                    var solutionLineTasks = $"Core {core}: ";
                    var solutionLine = "        ";

                    for (var task = 0; task < assignedTasks[core].Count; task++)
                    {
                        var assignedTask = assignedTasks[core][task];
                        var name = $"Node: {assignedTask.Index} ";
                        solutionLineTasks += $"{name,-15}";

                        var start = assignedTask.Start;
                        var duration = assignedTask.Duration;
                        var solutionTemp = $"[{start}, {start + duration}]";

                        solutionLine += $"{solutionTemp,-15}";
                    }

                    solutionLine += "\n";
                    solutionLineTasks += "\n";

                    output += solutionLineTasks;
                    output += solutionLine;
                }

                Console.WriteLine($"Optimal Schedule Length: {solver.ObjectiveValue}");
                Console.Write(output);
            }
        }

        private static List<Cpu> InitializeCpus(XmlDocument parsedCpu)
        {
            return parsedCpu.GetElementsByTagName("Cpu").Cast<XmlNode>()
                .Select(cpu => new Cpu(cpu)).ToList();
        }
    }
}