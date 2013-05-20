using System.Collections.Generic;
using System.Xml.Serialization;

namespace BolePlus.Client.Configuration
{
    [XmlRoot(ElementName = "Root",Namespace="")]
    public class CityConfiguration
    {
        [XmlArray(ElementName = "States"), XmlArrayItem(ElementName = "State")]
        public List<StateCollection> StateCollection { get; set; }
    }

    public class StateCollection
    {
        [XmlArray(ElementName = "Cities"), XmlArrayItem(ElementName = "City")]
        public List<CityItem> CityCollection { get; set; }

        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    public class CityItem
    {
        [XmlText]
        public string City { get; set; }

        [XmlAttribute("checked")]
        public bool Checked { get; set; }
    }
}
