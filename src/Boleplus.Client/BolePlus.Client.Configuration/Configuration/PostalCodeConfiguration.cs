using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BolePlus.Client.Configuration
{
    [XmlRoot(ElementName = "Root", Namespace = "")]
    public class PostalCodeConfiguration
    {
        [XmlArray(ElementName = "States"), XmlArrayItem(ElementName = "State")]
        public List<StateItem> StateCollection { get; set; }

        private Dictionary<string, StateItem> _satetDic;

        [XmlIgnore]
        public Dictionary<string, StateItem> StateDic
        {
            get
            {
                if (this._satetDic == null)
                {
                    this._satetDic = this.StateCollection.ToDictionary(i => i.Code, i => i);
                }

                return this._satetDic;
            }

        }
    }

    public class StateItem
    {
        [XmlArray(ElementName = "Cities"), XmlArrayItem(ElementName = "City")]
        public List<PostalCodeItem> PostalCodeCollection { get; set; }

        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
    }

    public class PostalCodeItem
    {
        public string Name { get; set; }

        public string State { get; set; }

        public string Code { get; set; }

        public bool Checked { get; set; }
    }
}