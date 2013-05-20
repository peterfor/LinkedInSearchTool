using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace BolePlus.Client.LinkedIn.Mongo
{
    public class MongoProfile
    {
        public MongoProfile()
        {
            this.CompanyId = new ObjectId("5105011878ab98361700000a");
            this.IdNumber = null;
            this.IdType = null;
            this.HireState = null;
            this.Gender = null;
            this.NationalityId = null;
            this.ResidencyId = null;
            this.QQ = null;
            this.Source = null;
            this.Wangwang = null;
            this.Emails = new List<string>();
            this.Phones = new Phone();
            this.LangProfiles = new  LangProfiles();
            this.ProfileUpdateTime = BsonDateTime.Create(DateTime.Now);
            this.MaritalStatus = null;
            this.DOB = null;
            this.CurrentLocationId = null;
            this.WebSites = new List<WebSite>();
            this.RelationShips = new RelationShip();
        }

        public BsonInt32 AutoId { get; set; }

        public ObjectId CompanyId { get; set; }

        public BsonString Source { get; set; }

        public BsonDateTime ProfileUpdateTime { get; set; }

        public BsonString IdType { get; set; }

        public BsonString IdNumber { get; set; }

        public BsonString MaritalStatus { get; set; }

        public BsonString HireState { get; set; }

        public BsonDateTime DOB { get; set; }

        public BsonString Gender { get; set; }

        public Object NationalityId { get; set; }

        public Object ResidencyId { get; set; }

        public Object CurrentLocationId { get; set; }

        public BsonString QQ { get; set; }

        public BsonString Wangwang { get; set; }

        public RelationShip RelationShips { get; set; }

        public List<string> Emails { get; set; }

        public List<WebSite> WebSites { get; set; }

        public Phone Phones { get; set; }

        public LangProfiles LangProfiles { get; set; }
    }

    public class WebSite
    {
        public WebSite()
        {
            this.Description = null;
            this.Url = null;
        }

        public string Description { get; set; }

        public string Url { get; set; }
    }

    public class LangProfiles
    {
        public LangProfiles()
        {
            this.EN = new LangProfile();
        }

        public LangProfile EN { get; set; }
    }

    public class Phone
    {
        public Phone()
        {
            this.Cell = new List<string>();
            this.Home = new List<string>();
            this.Work = new List<string>();
        }

        public List<string> Cell { get; set; }

        public List<string> Home { get; set; }

        public List<string> Work { get; set; }
    }

    public class RelationShip
    {
        public RelationShip()
        {
            this.FollowedUsers = new BsonArray();
            this.Projects = new BsonArray();
            this.Positions = new BsonArray();
        }

        public BsonArray FollowedUsers { get; set; }

        public BsonArray Projects { get; set; }

        public BsonArray Positions { get; set; }
    }

    public class CareerObjective
    {
        public CareerObjective()
        {
            this.ExpectEmploymentTypes = new BsonArray();
            this.Description = null;
            this.AvailableTime = null;
            this.CurrentPosition = null;
            this.CurrentSalaryText = null;
            this.CurrentStatus = null;
            this.CurrentWorkLocation = null;
            this.ExpectIndustries = new BsonArray();
            this.ExpectPositions = new BsonArray();
            this.ExpectSalaryText = null;
            this.ExpectWorkLocations = new BsonArray();
        }

        public BsonArray ExpectEmploymentTypes { get; set; }

        public BsonString Description { get; set; }

        public BsonString AvailableTime { get; set; }

        public BsonString CurrentPosition { get; set; }

        public BsonString CurrentWorkLocation { get; set; }

        public BsonArray ExpectIndustries { get; set; }

        public BsonArray ExpectPositions { get; set; }

        public BsonArray ExpectWorkLocations { get; set; }

        public BsonString CurrentStatus { get; set; }

        public BsonString ExpectSalaryText { get; set; }

        public BsonString CurrentSalaryText { get; set; }
    }

    public class LangProfile
    {
        public LangProfile()
        {
            this.Name = null;
            this.Expertises = null;
            this.ProfileUpdateTime = BsonDateTime.Create(DateTime.Now);
            this.Address = null;
            this.CareerObjective = new CareerObjective();
            this.Comments = null;
            this.CurrentLocation = null;
            this.Keywords = null;
            this.FirstName = null;
            this.LastName = null;
            this.WorkExps = new  List<WorkExp>();
            this.ProjectExps = new  List<ProjectExp>();
            this.LangExps = new  List<LangExp>();
            this.EduExps = new  List<EduExp>();
            this.TrainingExps = new  List<TrainingExp>();
            this.CertificateExps = new  List<CertificateExp>();
            this.Skills = new List<Skill>();
        }

        public BsonDateTime ProfileUpdateTime { get; set; }

        public String Name { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public BsonString Expertises { get; set; }

        public BsonString Address { get; set; }

        public BsonString Residency { get; set; }

        public BsonString CurrentLocation { get; set; }

        public BsonString SelfEvaluation { get; set; }

        public BsonString Comments { get; set; }

        public BsonString Keywords { get; set; }

        public CareerObjective CareerObjective { get; set; }

        public List<WorkExp> WorkExps { get; set; }

        public List<ProjectExp> ProjectExps { get; set; }

        public List<LangExp> LangExps { get; set; }

        public List<EduExp> EduExps { get; set; }

        public List<TrainingExp> TrainingExps { get; set; }

        public List<CertificateExp> CertificateExps { get; set; }

        public List<Skill> Skills { get; set; }
    }

    public class WorkExp
    {
        public WorkExp()
        {
            this.Description = null;
            this.IndustryId = null;
            this.Position = null;
            this.Department = null;
            this.Industry = null;
            this.PositionId = null;
            this.Achievements = null;
            this.Reference = null;
            this.CompanyName = null;
            this.Subordinates = null;
            this.ReportTo = null;
            this.CompanySize = null;
            this.Responsibilities = null;
            this.Overseas = null;
            this.CompanyType = null;
            this.ReasonForLeaving = null;
        }

        public BsonString Description { get; set; }

        public Object IndustryId { get; set; }

        public BsonString Position { get; set; }

        public BsonString Department { get; set; }

        public Object PositionId { get; set; }

        public BsonString Achievements { get; set; }

        public BsonString Reference { get; set; }

        public BsonString CompanyName { get; set; }

        public BsonString Industry { get; set; }

        public BsonString Subordinates { get; set; }

        public BsonString ReportTo { get; set; }

        public BsonString CompanySize { get; set; }

        public BsonString Responsibilities { get; set; }

        //bool
        public Object Overseas { get; set; }

        public BsonString CompanyType { get; set; }

        public BsonString ReasonForLeaving { get; set; }

        public BsonDateTime StartTime { get; set; }

        public BsonDateTime EndTime { get; set; }
    }

    public class ProjectExp
    {
        public ProjectExp()
        {
            this.Description = null;
            this.Responsibilities = null;
            this.Name = null;
            this.Role = null;
        }

        public BsonString Description { get; set; }

        public BsonString Responsibilities { get; set; }

        public BsonString Name { get; set; }

        public BsonString Role { get; set; }

        public BsonDateTime StartTime { get; set; }

        public BsonDateTime EndTime { get; set; }
    }

    public class LangExp
    {
        public LangExp()
        {
            this.Description = null;
            this.Language = null;
            this.Level = 4;
        }        

        public BsonString Description { get; set; }

        public BsonString Language { get; set; }

        public BsonInt32 Level { get; set; }
    }

    public class TrainingExp
    {
        public TrainingExp()
        {
            this.Description = null;
            this.FacilityName = null;
            this.Curriculums = null;
            this.CertName = null;
            this.TrainingLocation = null;
            this.TrainingLocationId = null;
            this.CertType = null;
            this.CertDuration = null;
        }

        public BsonString Description { get; set; }

        public BsonString FacilityName { get; set; }

        public BsonString Curriculums { get; set; }

        public BsonString CertName { get; set; }

        public BsonString TrainingLocation { get; set; }

        public Object TrainingLocationId { get; set; }

        public BsonString CertType { get; set; }

        public BsonString CertDuration { get; set; }

        public BsonDateTime StartTime { get; set; }

        public BsonDateTime EndTime { get; set; }
    }

    public class EduExp
    {
        public EduExp()
        {
            this.Description = null;
            this.SchoolName = null;
            this.Degree = null;
            this.DegreeId = null;
            this.Major = null;
            this.MajorId = null;
            this.Location = null;
            this.Reference = null;
            this.LocationId = null;;
            this.Overseas = null;
        }

        public BsonString Description { get; set; }

        public BsonString SchoolName { get; set; }

        public BsonString Degree { get; set; }

        public Object DegreeId { get; set; }

        public BsonString Major { get; set; }

        public Object MajorId { get; set; }

        public BsonString Reference { get; set; }

        public BsonString Location { get; set; }

        public Object LocationId { get; set; }

        //bool
        public Object Overseas { get; set; }

        public BsonDateTime StartTime { get; set; }

        public BsonDateTime EndTime { get; set; }
    }

    public class CertificateExp
    {
        public CertificateExp()
        {
            this.Description = null;
            this.CertName = null;
            this.CertAuthority = null;
         }

        public BsonString Description { get; set; }

        public BsonString CertName { get; set; }

        public BsonString CertAuthority { get; set; }

        public BsonDateTime StartTime { get; set; }

        public BsonDateTime EndTime { get; set; }
    }

    public class Skill
    {
        public Skill()
        {
            this.Name = null;
            this.Level = null;
        }        

        public BsonString Name { get; set; }

        public BsonInt32 Level { get; set; }
    }
}
