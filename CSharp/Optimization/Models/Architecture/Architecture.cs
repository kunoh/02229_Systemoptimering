using System.Collections.Generic;
using System.Xml;

namespace Optimization.Models.Architecture
{
    /// <summary>
    /// The architecture has at least one CPU and at least one Core for each CPU.
    /// </summary>
    public class Architecture
    {
        public Architecture(XmlDocument document)
        {
            var cpuNodes = document.GetElementsByTagName("Cpu");
            this.Cpus = new List<Cpu>();
            
            foreach (XmlNode node in cpuNodes)
            {
                this.Cpus.Add(new Cpu(node));
            }
        }

        public List<Cpu> Cpus { get; }
    }
}