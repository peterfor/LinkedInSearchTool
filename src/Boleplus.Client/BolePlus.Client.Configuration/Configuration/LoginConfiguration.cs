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
    public class LoginConfiguration
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Distance { get; set; }

        public string SortBy { get; set; }

        public int PostCodeCount { get; set; }
    }   
}
