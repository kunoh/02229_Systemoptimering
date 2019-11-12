using System;
using System.Linq;
using System.Xml;
using Optimization.Models.Application;
using Optimization.Models.Architecture;

namespace Optimization
{
    public static class XmlParser
    {
        /// <summary>
        /// Parses an XML document containing an Application into a data object
        /// </summary>
        /// <param name="document">Loaded xml document containing the application</param>
        /// <returns><see cref="Application"/> Object</returns>
        public static Application ParseApplication(XmlDocument document)
        {
            var tasks = (from XmlNode task in document.GetElementsByTagName("Node") 
            select new Task
            {
                Id = GetValue("Id", task),
                Name = GetValue("Name", task),
                Wcet = GetIntValue("WCET", task),
                Period = GetIntValue("Period", task),
                Deadline = GetIntValue("Deadline", task),
                MaxJitter = GetIntValue("MaxJitter", task),
                Offset = GetIntValue("Offset", task),
                CpuId = GetIntValue("CpuId", task),
                CoreId = GetIntValue("CoreId", task)
            }).ToList();
            
            var chains = (from XmlNode chain in document.GetElementsByTagName("Chain") 
            select new Chain
            {
                Budget = GetIntValue("Budget", chain), 
                Priority = GetIntValue("Priority", chain), 
                Name = GetValue("Name", chain), 
                Runnables = chain.ChildNodes.Cast<XmlNode>()
                    .Select(runnable => runnable.Attributes.GetNamedItem("Name").Value).ToList()
            }).ToList();

            return new Application { Tasks = tasks, Chains = chains };
        }

        public static Architecture ParseArchitecture(XmlDocument document)
        {
            var cpus = (from XmlNode cpu in document.GetElementsByTagName("Cpu") select new Cpu
            {
                Id = cpu.Attributes.GetNamedItem("Id").Value, 
                Cores = cpu.ChildNodes.Cast<XmlNode>()
                    .Select(core => new Core
                    {
                        Id = core.Attributes.GetNamedItem("Id").Value, 
                        MacroTick = Convert.ToInt32(core.Attributes.GetNamedItem("MacroTick").Value)
                    }).ToList()
            }).ToList();

            return new Architecture { Cpus = cpus };
        }
        
        private static string GetValue(string attr, XmlNode node)
        {
            return node.Attributes.GetNamedItem(attr).Value;
        }

        private static int GetIntValue(string attr, XmlNode node)
        {
            return Convert.ToInt32(node.Attributes.GetNamedItem(attr).Value);
        }
    }
}