using System.Xml;

namespace OptimizationExercise
{
    public class XmlParser
    {
        public XmlParser(string documentName)
        {
            this.Document = new XmlDocument();
            this.Document.Load(documentName);
        }

        public XmlDocument Document { get; }
    }
}