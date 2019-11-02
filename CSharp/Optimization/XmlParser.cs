using System.IO;
using System.Xml;

namespace OptimizationExercise
{
    public class XmlParser
    {
        public XmlParser(string path, string fileExtension)
        {
            var documentName = Directory.GetFiles(path, $"*.{fileExtension}")[0];
            this.Document = new XmlDocument();
            this.Document.Load(documentName);
        }

        public XmlDocument Document { get; }
    }
}