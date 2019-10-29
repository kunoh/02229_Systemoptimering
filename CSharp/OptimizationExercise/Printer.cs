using System;
using System.Collections.Generic;
using OptimizationExercise.Models;
using Google.OrTools.Sat;
using System.Linq;

namespace OptimizationExercise
{
    public class Printer : CpSolverSolutionCallback
    {
        Dictionary<(int, int), Task> tasks;
        int coreCount, nodeCount;
        Scenario scenario;

        public Printer(Dictionary<(int, int), Task> tasks, int coreCount, int nodeCount, Scenario scenario)
        {
            this.tasks = tasks;
            this.coreCount = coreCount;
            this.nodeCount = nodeCount;
            this.scenario = scenario;
        }

        public override void OnSolutionCallback()
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
                    assignedTasks[core].Add(new AssignedTask
                    {
                        Start = Value(tasks[(core, node)].Start),
                        Core = core,
                        Index = scenario.Graph.Nodes[node].Name,
                        Duration = scenario.Graph.Nodes[node].Wcet,
                        IsActive = Value(tasks[(core, node)].IsActive)
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
                    if (assignedTask.IsActive == 1)
                    {
                        var name = $"Node: {assignedTask.Index} ";
                        solutionLineTasks += $"{name,-15}";
                    }

                    var solutionTemp = string.Empty;
                    var start = assignedTask.Start;
                    var duration = assignedTask.Duration;
                    if (assignedTask.IsActive == 1)
                    {
                        solutionTemp = $"[{start}, {start + duration}]";
                        solutionLine += $"{solutionTemp,-15}";
                    }
                }

                solutionLine += "\n";
                solutionLineTasks += "\n";

                output += solutionLineTasks;
                output += solutionLine;
            }

            Console.WriteLine($"Optimal Schedule Length: {ObjectiveValue()}");
            Console.Write(output);
        }
    }
    
}