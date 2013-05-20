using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BolePlus.Client.LinkedIn
{
    [DataContract]
    public class CollegeProfile
    {
        [DataMember(Name = "people")]
        public CollecgeProfileDetail CollecgeProfileDetail { get; set; }
    }

    [DataContract]
    public class CollecgeProfileDetail
    {
        [DataMember(Name = "_total")]
        public int TotalCount { get; set; }

        [DataMember(Name = "_count")]
        public int Count { get; set; }

        [DataMember(Name = "values")]
        public List<CollegeProfileModel> ProfileCollection { get; set; }
    }

    [DataContract]
    public class CollegeProfileModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "fullName")]
        public string Name { get; set; }

        [DataMember(Name = "publicProfileUrl")]
        public string ViewProfileUrl { get; set; }
    }
}
