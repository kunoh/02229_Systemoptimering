using System;
using Google.OrTools.Sat;

namespace OptimizationExercise
{
    public class SolutionPrinter : CpSolverSolutionCallback
    {
        public SolutionPrinter()
        {
            
        }

        public override void OnSolutionCallback()
        {
            Console.WriteLine("Solution found");
        }
    }
}