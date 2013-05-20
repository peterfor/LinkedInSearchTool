using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace BolePlus.Client.LinkedIn.Json
{
    [DataContract]
    public class LinkedInProfile
    {
        [DataMember(Name = "content")]
        public ProfileContent Content { get; set; }
    }

    [DataContract]
    public class ProfileContent
    {
        [DataMember]
        public Summary Summary { get; set; }

        [DataMember]
        public BasicInfo BasicInfo { get; set; }

        [DataMember]
        public Skill Skills { get; set; }

        [DataMember]
        public ContactInfo ContactInfo { get; set; }

        [DataMember]
        public Experience Experience { get; set; }

        [DataMember]
        public Language Languages { get; set; }

        [DataMember]
        public Discovery Discovery { get; set; }

        [DataMember]
        public Education Education { get; set; }

        [DataMember]
        public Project Projects { get; set; }

        [DataMember(Name = "global_requestParams")]
        public GobalInfo GobalInfo { get; set; }
    }

    [DataContract]
    public class GobalInfo
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
    }

    [DataContract]
    public class Project
    {
        [DataMember(Name = "projectsMpr")]
        public ProjectMpr ProjectMpr { get; set; }
    }

    [DataContract]
    public class ProjectMpr
    {
        [DataMember(Name = "projects")]
        public List<ProjectDetail> Projects { get; set; }
    }

    [DataContract]
    public class ProjectDetail
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "desc")]
        public string Description { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "enddate_iso")]
        public string EndDate { get; set; }

        [DataMember(Name = "startdate_iso")]
        public string StartDate { get; set; }
    }

    [DataContract]
    public class Summary
    {
        [DataMember(Name = "summary")]
        public SummaryDetail SummaryDetail { get; set; }

    }

    [DataContract]
    public class SummaryDetail
    {
        [DataMember(Name = "specialties_lb")]
        public string Specialties { get; set; }

        [DataMember(Name = "summary_lb")]
        public string SummaryMessage { get; set; }
    }

    [DataContract]
    public class Skill
    {
        [DataMember(Name = "skillsMpr")]
        public SkillMpr SkillMpr { get; set; }
    }

    [DataContract]
    public class SkillMpr
    {
        [DataMember(Name = "skills")]
        public List<SkillDetail> Skills { get; set; }

        [DataMember(Name = "recipientFullName")]
        public string FullName { get; set; }

        [DataMember(Name = "recipientFirstName")]
        public string FirstName { get; set; }

        [DataMember(Name = "recipientLastName")]
        public string LastName { get; set; }
    }

    [DataContract]
    public class SkillDetail
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class BasicInfo
    {
        [DataMember(Name = "basic_info")]
        public BasicInfoDetail BasicInfoDetail { get; set; }

    }

    [DataContract]
    public class BasicInfoDetail
    {
        [DataMember(Name = "fullname")]
        public string FullName { get; set; }

        [DataMember(Name = "fmt_location")]
        public string CurrentLocation { get; set; }
    }

    [DataContract]
    public class ContactInfo
    {
        [DataMember(Name = "contact_info")]
        public ContactInfoDetail ContactInfoDetail { get; set; }
    }

    [DataContract]
    public class ContactInfoDetail
    {
        [DataMember(Name = "emails")]
        public List<LinkedInEmail> LinkedInEmails { get; set; }

        [DataMember(Name = "websites")]
        public List<LinkedInWebSite> LinkedInWebSites { get; set; }
    }

    [DataContract]
    public class LinkedInEmail
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }
    }

    [DataContract]
    public class LinkedInWebSite
    {
        [DataMember(Name = "URL")]
        public string Url { get; set; }

        [DataMember(Name = "fmt_type")]
        public string Type { get; set; }
    }

    [DataContract]
    public class Discovery
    {
        [DataMember(Name = "discovery")]
        public DiscoveryDetail DiscoveryDetail { get; set; }
    }

    [DataContract]
    public class DiscoveryDetail
    {
        [DataMember(Name = "viewee")]
        public Name Name { get; set; }
    }

    [DataContract]
    public class Name
    {
          [DataMember(Name = "firstName")]
        public string FirstName { get; set; }

          [DataMember(Name = "lastName")]
        public string LastName { get; set; }
    }

    [DataContract]
    public class Experience
    {
        [DataMember(Name = "positionsMpr")]
        public PositionsMpr PositionMpr { get; set; }
    }

    [DataContract]
    public class PositionsMpr
    {
        [DataMember(Name = "positions")]
        public List<Position> Positions { get; set; }
    }

    [DataContract]
    public class Position
    {
        [DataMember(Name = "title_highlight")]
        public string PositionName { get; set; }

        [DataMember(Name = "companyName")]
        public string Company { get; set; }

        [DataMember(Name = "startdate_iso")]
        public string StartDate { get; set; }

        [DataMember(Name = "enddate_iso")]
        public string EndDate { get; set; }

        [DataMember(Name = "summary_lb")]
        public string Description { get; set; }
    }

    [DataContract]
    public class Language
    {
        [DataMember(Name = "languages")]
        public LanguageData LanguageData { get; set; }
    }

    [DataContract]
    public class LanguageData
    {
        [DataMember(Name = "languagesData")]
        public List<LanguageDetail> LanguageDetails { get; set; }
    }

    [DataContract]
    public class LanguageDetail
    {
        [DataMember(Name = "displayName")]
        public string Name { get; set; }

    }

    [DataContract]
    public class Education
    {
        [DataMember(Name = "educationsMpr")]
        public EducationsMpr EducationsMpr { get; set; }
    }

    [DataContract]
    public class EducationsMpr
    {
        [DataMember(Name = "educations")]
        public List<EducationDetail> EducationDetails { get; set; }
    }

    [DataContract]
    public class EducationDetail
    {
        [DataMember(Name = "fieldOfStudy")]
        public string Study { get; set; }

        [DataMember(Name = "schoolName")]
        public string SchoolName { get; set; }

        [DataMember(Name = "enddate_iso")]
        public string EndDate { get; set; }

        [DataMember(Name = "startdate_iso")]
        public string StartDate { get; set; }

        [DataMember(Name = "degree")]
        public string Degree { get; set; }

        [DataMember(Name = "notes")]
        public string Description { get; set; }
    }
}
