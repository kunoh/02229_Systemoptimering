using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.Sat;
using JobShopProblem.Models;

namespace JobShopProblem
{
    public static class JobShopProblem
    {
        public static void Main(string[] args)
        {
            // -- CREATE THE MODEL --
            var model = new CpModel();

            // -- SET UP THE DATA --
            var jobs = CreateJobsList();

            // Number of machines
            var machinesCount = 1 + jobs.Max(job => job.Max(task => task.Item1));
            
            // List of machines
            var machines = new List<int>();
            InitMachinesList(machinesCount, machines);

            // Total duration of a single task
            var horizon = jobs.Sum(job => job.Sum(task => task.Item2));
            
            // -- ADD VARIABLES TO MODEL --
            // Dictionary of all tasks using a tuple of job and task as key 
            var tasks = new Dictionary<(int, int), Task>();
            
            // List containing each machines' intervals
            // Interval is time between tasks
            var machineIntervals = new List<List<IntervalVar>>();
            InitMachineIntervalsList(machinesCount, machineIntervals);

            // Add all variables to the model
            AddModelVariables(jobs, model, horizon, tasks, machineIntervals);
            
            // Add all constraints to the model
            AddConstraints(machines, model, machineIntervals, jobs, tasks, horizon);

            // Solve the model
            var solver = new CpSolver();
            var status = solver.Solve(model);

            // If an optimal solution is found, print it
            if (status == CpSolverStatus.Optimal)
            {
                PrintSolution(
                    machinesCount, 
                    AssignTasks(machinesCount, jobs, solver, tasks), 
                    solver);
            }
        }

        /// <summary>
        /// Create the list of assigned tasks, used to print the solution
        /// </summary>
        /// <param name="machinesCount">Amount of machines</param>
        /// <param name="jobs">List of jobs</param>
        /// <param name="solver">The cp solver used to solve the model</param>
        /// <param name="tasks">Dictionary of tasks</param>
        /// <returns>List of <see cref="AssignedTask"/> objects</returns>
        private static List<List<AssignedTask>> AssignTasks(
            int machinesCount, 
            List<List<(int, int)>> jobs, 
            CpSolver solver, 
            Dictionary<(int, int), Task> tasks)
        {
            var assignedTasks = new List<List<AssignedTask>>();

            for (var machine = 0; machine < machinesCount; machine++)
            {
                assignedTasks.Add(new List<AssignedTask>());
            }

            for (var job = 0; job < jobs.Count; job++)
            {
                for (var task = 0; task < jobs[job].Count; task++)
                {
                    var machine = jobs[job][task].Item1;
                    assignedTasks[machine].Add(new AssignedTask
                    {
                        Start = solver.Value(tasks[(job, task)].Start),
                        Job = job,
                        Index = task,
                        Duration = jobs[job][task].Item2
                    });
                }
            }

            return assignedTasks;
        }

        /// <summary>
        /// Add the constraints to the model.
        /// This tells the model how to solve the schedule.
        /// </summary>
        /// <param name="machines">List of machines</param>
        /// <param name="model">The CpModel to which the constraints are added</param>
        /// <param name="machineIntervals">List of Interval vars for each machine</param>
        /// <param name="jobs">List of jobs</param>
        /// <param name="tasks">Dictionary of all tasks</param>
        /// <param name="horizon">Largest duration of single task</param>
        private static void AddConstraints(
            List<int> machines, 
            CpModel model, 
            List<List<IntervalVar>> machineIntervals, 
            List<List<(int, int)>> jobs, 
            Dictionary<(int, int), Task> tasks,
            int horizon)
        {
            // Disjunctive
            // Ensure tasks aren't planned at the same time
            for (var machine = 0; machine < machines.Count; machine++)
            {
                model.AddNoOverlap(machineIntervals[machine]);
            }

            // Precedence
            // Ensure tasks aren't planned in the wrong order
            for (var job = 0; job < jobs.Count; job++)
            {
                for (var task = 0; task < jobs[job].Count - 1; task++)
                {
                    model.Add(tasks[(job, task + 1)].Start >= tasks[(job, task)].End);
                }
            }

            // Makespan
            // Ensure the duration is minimized
            var makespan = model.NewIntVar(0, horizon, "makespan");
            model.AddMaxEquality(makespan,
                jobs.Select((t, job) => tasks[(job, t.Count - 1)].End).ToList());
            model.Minimize(makespan);
        }

        private static void PrintSolution(int machinesCount, List<List<AssignedTask>> assignedTasks, CpSolver solver)
        {
            var output = string.Empty;

            for (var machine = 0; machine < machinesCount; machine++)
            {
                assignedTasks[machine] = assignedTasks[machine].OrderBy(task => task.Start).ToList();
                var solutionLineTasks = $"Machine {machine}: ";
                var solutionLine = "            ";

                for (var task = 0; task < assignedTasks[machine].Count; task++)
                {
                    var assignedTask = assignedTasks[machine][task];
                    var name = $"job_{assignedTask.Job}_{assignedTask.Index}";
                    solutionLineTasks += $"{name,-10}";

                    var start = assignedTask.Start;
                    var duration = assignedTask.Duration;
                    var solutionTemp = $"[{start}, {start + duration}]";

                    solutionLine += $"{solutionTemp,-10}";
                }

                solutionLine += "\n";
                solutionLineTasks += "\n";

                output += solutionLineTasks;
                output += solutionLine;
            }

            Console.WriteLine($"Optimal Schedule Length: {solver.ObjectiveValue}");
            Console.Write(output);
        }

        /// <summary>
        /// Add the variables to the <see cref="CpModel"/>.
        /// </summary>
        /// <param name="jobs"></param>
        /// <param name="model"></param>
        /// <param name="horizon"></param>
        /// <param name="tasks"></param>
        /// <param name="machineIntervals"></param>
        private static void AddModelVariables(
            List<List<(int, int)>> jobs, 
            CpModel model, 
            int horizon, 
            Dictionary<(int, int), Task> tasks, 
            List<List<IntervalVar>> machineIntervals)
        {
            for (var job = 0; job < jobs.Count; job++)
            {
                for (var task = 0; task < jobs[job].Count; task++)
                {
                    var machine = jobs[job][task].Item1;
                    var duration = jobs[job][task].Item2;
                    var suffix = $"_{job}_{task}";

                    var start = model.NewIntVar(0, horizon, $"start{suffix}");
                    var end = model.NewIntVar(0, horizon, $"end{suffix}");
                    var interval = model.NewIntervalVar(start, duration, end, $"interval{suffix}");

                    tasks[(job, task)] = new Task
                    {
                        Start = start,
                        End = end,
                        Interval = interval
                    };

                    machineIntervals[machine].Add(interval);
                }
            }
        }

        /// <summary>
        /// Used to initialize the list of machines
        /// </summary>
        /// <param name="machinesCount">Amount of machines</param>
        /// <param name="machines">List to initialize</param>
        private static void InitMachinesList(int machinesCount, List<int> machines)
        {
            for (var machine = 0; machine < machinesCount; machine++)
            {
                machines.Add(machine);
            }
        }

        /// <summary>
        /// Used to initialize the list of IntervalVars for each machine
        /// </summary>
        /// <param name="machinesCount">Amount of machines</param>
        /// <param name="machineIntervals">List to initialize</param>
        private static void InitMachineIntervalsList(int machinesCount, List<List<IntervalVar>> machineIntervals)
        {
            for (var machine = 0; machine < machinesCount; machine++)
            {
                machineIntervals.Add(new List<IntervalVar>());
            }
        }

        /// <summary>
        /// Initializes the array of jobs to schedule
        /// </summary>
        /// <returns>An array of jobs</returns>
        private static List<List<(int,int)>> CreateJobsList()
        {
            return new List<List<(int, int)>>
            {
                new List<(int, int)>() {(0, 3), (1, 2), (2, 2) },
                new List<(int, int)>() { (0, 2), (2, 1), (1, 4) },
                new List<(int, int)>() { (1, 4), (2, 3) }
            };
        }
    }
}