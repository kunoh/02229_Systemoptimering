using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace OptimizationExercise.Models.Configuration
{
    public class Cpu
    {
        public Cpu(XmlNode cpuNode)
        {
            this.Id = Id = cpuNode.Attributes.GetNamedItem("Id").Value;
            this.Cores = cpuNode.ChildNodes.Cast<XmlNode>()
                .Select(core => new Core
                {
                    Id = core.Attributes.GetNamedItem("Id").Value
                }).ToList();
        }
        
        public string Id { get; set; }
        public List<Core> Cores { get; set; }
    }
}