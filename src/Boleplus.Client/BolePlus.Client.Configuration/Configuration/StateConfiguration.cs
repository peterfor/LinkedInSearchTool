using System.Collections.Generic;
using System.Xml.Serialization;

namespace BolePlus.Client.Configuration
{
    [XmlRoot(ElementName = "Root",Namespace="")]
    public class StateConfiguration
    {
        [XmlArray(ElementName = "Codes"), XmlArrayItem(ElementName = "Code")]
        public List<State> PostalCodeCollection { get; set; }
    }

    public class State
    {
        [XmlText]
        public string Code { get; set; }
    }
}
