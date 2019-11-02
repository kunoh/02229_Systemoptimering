using System.Collections.Generic;
using System.Xml;

namespace Optimization.Models.Application
{
    /// <summary>
    /// The application has tasks and chains. A task is tagged with "Node"
    /// </summary>
    public class Application
    {
        public Application(XmlDocument document)
        {
            var taskNodes = document.GetElementsByTagName("Node");
            var chainNodes = document.GetElementsByTagName("Chain");
            
            this.Tasks = new List<Task>();
            this.Chains = new List<Chain>();

            foreach (XmlNode taskNode in taskNodes)
            {
                this.Tasks.Add(new Task(taskNode));
            }

            foreach (XmlNode chainNode in chainNodes)
            {
                this.Chains.Add(new Chain(chainNode));
            }
        }

        public List<Task> Tasks { get; }
        public List<Chain> Chains { get; }
    }
}
