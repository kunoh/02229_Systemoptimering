using System;
using Google.OrTools.Sat;

namespace OptimizationExercise
{
    public class Printer : CpSolverSolutionCallback
    {
        public Printer()
        {
            Console.WriteLine("New solution");
        }

        public override void OnSolutionCallback()
        {
            Console.WriteLine("New Solution");
        }
    }
}