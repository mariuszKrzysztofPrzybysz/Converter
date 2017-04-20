using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Converter.Models
{
    [Serializable]
    public class Sentence
    {
        [XmlElement(ElementName = "word")] public List<Word> Words = new List<Word>();
    }
}