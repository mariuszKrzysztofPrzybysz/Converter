using System;
using System.Xml.Serialization;

namespace Converter.Models
{
    [Serializable]
    public sealed class Word
    {
        [XmlText] public string Value;
    }
}