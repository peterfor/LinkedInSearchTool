using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace BolePlus.Client.LinkedIn
{
    [DataContract]
    public class LinkedInSearchResult
    {
        [DataMember(Name = "content")]
        public Content Content { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }
    }

    [DataContract]
    public class Content
    {
        [DataMember(Name = "voltron_unified_search_json")]
        public SearchJson SearchJson { get; set; }
    }

    [DataContract]
    public class SearchJson
    {
        [DataMember(Name = "search")]
        public Search Search { get; set; }
    }

    [DataContract]
    public class Search
    {
        [DataMember(Name = "results")]
        public List<Person> Results { get; set; }

        [DataMember(Name="jobs_i18n")]
        public string Job { get; set; }
    }

    [DataContract]
    public class Person
    {
        [DataMember(Name = "person")]
        public PersonDetail PersonDetail { get; set; }
    }

    [DataContract]
    public class PersonDetail
    {
        [DataMember(Name = "headline")]
        public string HeadLine { get; set; }

        [DataMember(Name = "isHeadless")]
        public bool IsHeadless { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "link_nprofile_view_4")]
        public string ViewProfileUrl { get; set; }
        
    }

}
