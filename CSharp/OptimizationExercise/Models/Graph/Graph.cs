using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OptimizationExercise.Models.Graph
{
    public class Graph
    {
        public Graph(XmlDocument document)
        {
            this.Nodes = document.GetElementsByTagName("Node").Cast<XmlNode>().Select(node => new Node
            {
                Id = node.Attributes.GetNamedItem("Id").Value,
                Name = node.Attributes.GetNamedItem("Name").Value,
                Wcet = Convert.ToInt32(node.Attributes.GetNamedItem("WCET").Value),
                Period = Convert.ToInt32(node.Attributes.GetNamedItem("Period").Value),
                Deadline = Convert.ToInt32(node.Attributes.GetNamedItem("Deadline").Value)
            }).ToList();

            this.TaskGraphs = document.GetElementsByTagName("TaskGraph").Cast<XmlNode>().Select(taskGraph =>
                new TaskGraph
                {
                    Name = taskGraph.Attributes.GetNamedItem("Name").Value,
                    Edges = taskGraph.ChildNodes.Cast<XmlNode>().Select(edge => new Edge
                    {
                        Source = edge.Attributes.GetNamedItem("Source").Value,
                        Destination = edge.Attributes.GetNamedItem("Dest").Value,
                        Cost = Convert.ToInt32(edge.Attributes.GetNamedItem("Cost").Value)
                    }).ToList()
                }).ToList();
        }
        
        public List<Node> Nodes { get; set; }
        public List<TaskGraph> TaskGraphs { get; set; }
    }
}