using System.Collections.Generic;
using System.Xml.Serialization;

namespace Converter.Models
{
    //[Serializable]
    [XmlRoot(ElementName = "text")]
    public class Text
    {
        [XmlElement(ElementName = "sentence")] public List<Sentence> Sentences = new List<Sentence>();
    }
}