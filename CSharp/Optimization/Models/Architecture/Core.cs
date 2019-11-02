using System;
using System.Xml;

namespace Optimization.Models.Architecture
{
    /// <summary>
    /// The Core element
    /// Attributed with an "Id" tag which is a unique integer within the parent element.
    /// It is also attributed with "MacroTick" tag.
    /// </summary>
    public class Core
    {
        public Core(XmlNode node)
        {
            this.Id = node.Attributes.GetNamedItem("Id").Value;
            this.MacroTick = Convert.ToInt32(node.Attributes.GetNamedItem("MacroTick").Value);
        }
        
        public string Id { get; }
        
        /// <summary>
        /// The attribute tag for a Core; integer;
        /// Indicates time in microseconds; shows the time granularity of the core scheduling.
        /// Preemption is possible through the MacroTick. Preemption is not allowed by assigning big 9999999 to microtick
        /// </summary>
        public int MacroTick { get; }
    }
}