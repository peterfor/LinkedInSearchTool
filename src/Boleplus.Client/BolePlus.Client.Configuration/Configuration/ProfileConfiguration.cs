using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Threading;
using System.Collections.Concurrent;

namespace BolePlus.Client.Configuration.Configuration
{
    [XmlRoot(ElementName = "Root", Namespace = "")]
    public class ProfileConfiguration
    {
        [XmlArray(ElementName = "Profiles"), XmlArrayItem(ElementName = "Profile")]
        public List<ProfileItem> ProfileCollection { get; set; }

        private Dictionary<int, ProfileItem> itemDic;

        [XmlIgnore]
        public Dictionary<int, ProfileItem> ItemDic
        {
            get
            {
                if (this.itemDic == null && this.ProfileCollection != null)
                {
                    this.itemDic = this.ProfileCollection.ToDictionary(i => i.Id, i => i);
                }

                return this.itemDic;
            }
        }

        private List<int> _profileItems;

        [XmlIgnore]
        public List<int> ProfileIdItems
        {
            get
            {
                if (this._profileItems == null)
                {
                    this._profileItems = new List<int>();
                    foreach (var item in this.ProfileCollection)
                    {
                        this._profileItems.Add(item.Id);
                    }
                }

                return this._profileItems;
            }
        }

        public void AddProfileId(int id)
        {
            if (this._profileItems != null)
            {
                this._profileItems.Add(id);
            }
        }

    }

    public class ProfileItem
    {
        public int Id { get; set; }

        public string ViewProfileUrl { get; set; }

        public string Name { get; set; }

        public bool Checked { get; set; }

        public bool HasViewed { get; set; }
    }
}
