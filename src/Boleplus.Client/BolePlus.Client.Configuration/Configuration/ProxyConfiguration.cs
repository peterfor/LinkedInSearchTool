using BolePlus.Client.Web.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BolePlus.Client.Configuration
{
    [XmlRoot(ElementName = "Root", Namespace = "")]
    public class ProxyConfiguration
    {
        [XmlArray(ElementName = "Proxies"), XmlArrayItem(ElementName = "Proxy")]
        public List<ProxyItem> ProxyCollection { get; set; }
    }

    public class ProxyItem
    {
        public string Ip { get; set; }

        public string Port { get; set; }

        [XmlIgnore]
        public RequestSession Session { get; set; }
    }
}
