using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using OptimizationExercise.Models;
using OptimizationExercise.Models.Configuration;
using OptimizationExercise.Models.Graph;

namespace OptimizationExercise
{
    public static class Program
    {
        private const string DirectoryName = "..\\..\\..\\Cases\\Case 4\\";
        private static readonly string GraphFileName = $"{DirectoryName}Case4.tsk";
        private static readonly string CpuFileName = $"{DirectoryName}Case4.cfg";

        public static void Main(string[] args)
        {
            var parsedCpu = new XmlParser(CpuFileName).Document;
            var parsedGraph = new XmlParser(GraphFileName).Document;
            
            var scenario = new Scenario
            {
                Cpus = InitializeCpus(parsedCpu),
                Graph = new Graph(parsedGraph)
            };
        }

        private static List<Cpu> InitializeCpus(XmlDocument parsedCpu)
        {
            return parsedCpu.GetElementsByTagName("Cpu").Cast<XmlNode>()
                .Select(cpu => new Cpu(cpu)).ToList();
        }
    }
}