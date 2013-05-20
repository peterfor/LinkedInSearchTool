using BolePlus.Client.LinkedIn.Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolePlus.Client.LinkedIn.DataAccessor
{
    public class ProfileDataAccessor
    {
        static ProfileDataAccessor()
        {
            //创建数据连接
            MongoServer server = MongoServer.Create(conn);
            //获取指定数据库
            Db = server.GetDatabase(dbName);
            //获取表
            MongoCollection = Db.GetCollection(tbName);

        }

        private static MongoDatabase Db { get; set; }

        /// <summary>
        /// 数据库连接
        /// </summary>
        private const string conn = "mongodb://127.0.0.1:27017";

       // private const string conn = "mongodb://boleplusdbo:boleplus123@42.120.7.130";
        /// <summary>
        /// 指定的数据库
        /// </summary>
        private const string dbName = "boleplusdb";
        /// <summary>
        /// 指定的表
        /// </summary>
        private const string tbName = "linkedinprofiles";

        private static MongoCollection MongoCollection { get; set; }

        public static BsonInt32 GetAutoId()
        {
            var collection = Db.GetCollection("seqs");
            var query = Query.EQ("_id", "profiles");
            var update = Update.Inc("seq", 1);
            var sortBy = SortBy.Null;
            var result = collection.FindAndModify(query, sortBy, update, true);
            return (BsonInt32)result.ModifiedDocument["seq"];
        }

        public static void Insert(MongoProfile model)
        {
           Console.WriteLine(MongoCollection.Count());
            MongoCollection.Insert(model);
        }

        public static bool Insert(BolePlus.Client.LinkedIn.Json.LinkedInProfile profile)
        {
            bool insertSucceed = true;
            try
            {
                MongoProfile model = new MongoProfile();
                if (profile.Content != null)
                {
                    model.LangProfiles = new LangProfiles();
                    var langProfile = model.LangProfiles.EN;

                    var content = profile.Content;
                    if (content.Summary != null && content.Summary.SummaryDetail != null)
                    {
                        langProfile.Expertises = content.Summary.SummaryDetail.Specialties;
                        langProfile.SelfEvaluation = content.Summary.SummaryDetail.SummaryMessage;
                    }
                    if (content.BasicInfo != null && content.BasicInfo.BasicInfoDetail != null)
                    {
                        langProfile.CurrentLocation = content.BasicInfo.BasicInfoDetail.CurrentLocation;
                        langProfile.Name = content.BasicInfo.BasicInfoDetail.FullName;
                    }
                    if (content.Skills != null && content.Skills.SkillMpr != null)
                    {
                        if (!string.IsNullOrEmpty(content.Skills.SkillMpr.FirstName))
                        {
                            langProfile.FirstName = content.Skills.SkillMpr.FirstName;
                        }
                        if (!string.IsNullOrEmpty(content.Skills.SkillMpr.LastName))
                        {
                            langProfile.LastName = content.Skills.SkillMpr.LastName;
                        }
                        if (!string.IsNullOrEmpty(content.Skills.SkillMpr.FullName) && string.IsNullOrEmpty(langProfile.Name))
                        {
                            langProfile.Name = content.Skills.SkillMpr.FullName;
                        }
                        if (content.Skills.SkillMpr.Skills != null)
                        {
                            foreach (var skill in content.Skills.SkillMpr.Skills)
                            {
                                var item = new Skill()
                                {
                                    Name = skill.Name
                                };

                                langProfile.Skills.Add(item);
                            }
                        }
                    }
                    if (content.ContactInfo != null && content.ContactInfo.ContactInfoDetail != null)
                    {
                        var contactInfo = content.ContactInfo.ContactInfoDetail;
                        if (contactInfo.LinkedInEmails != null)
                        {
                            foreach (var email in contactInfo.LinkedInEmails)
                            {
                                model.Emails.Add(email.Email);
                            }
                        }
                        if (contactInfo.LinkedInWebSites != null)
                        {
                            foreach (var website in contactInfo.LinkedInWebSites)
                            {
                                model.WebSites.Add(new WebSite()
                                {
                                    Description = website.Type,
                                    Url = website.Url
                                });
                            }
                        }
                    }
                    if (content.Experience != null && content.Experience.PositionMpr != null && content.Experience.PositionMpr.Positions != null)
                    {
                        foreach (var position in content.Experience.PositionMpr.Positions)
                        {
                            var workExp = new WorkExp()
                            {
                                CompanyName = position.Company,
                                Position = position.PositionName,
                                Description = position.Description
                            };

                            workExp.StartTime = ConvertToBsonDateTime(position.StartDate);
                            workExp.EndTime = ConvertToBsonDateTime(position.EndDate);
                            langProfile.WorkExps.Add(workExp);
                        }
                    }
                    if (content.Languages != null && content.Languages.LanguageData != null && content.Languages.LanguageData.LanguageDetails != null)
                    {
                        foreach (var lang in content.Languages.LanguageData.LanguageDetails)
                        {
                            var language = new LangExp()
                            {
                                Language = lang.Name
                            };
                        }
                    }
                    if (content.Discovery != null && content.Discovery.DiscoveryDetail != null && content.Discovery.DiscoveryDetail.Name != null)
                    {
                        var nameInstance = content.Discovery.DiscoveryDetail.Name;
                        if (!string.IsNullOrEmpty(nameInstance.FirstName))
                        {
                            langProfile.FirstName = nameInstance.FirstName;
                        }
                        if (!string.IsNullOrEmpty(nameInstance.LastName))
                        {
                            langProfile.LastName = nameInstance.LastName;
                        }
                    }
                    if (content.Education != null && content.Education.EducationsMpr != null && content.Education.EducationsMpr.EducationDetails != null)
                    {
                        foreach (var edu in content.Education.EducationsMpr.EducationDetails)
                        {
                            var eduExp = new EduExp()
                            {
                                Degree = edu.Degree,
                                Major = edu.Study,
                                SchoolName = edu.SchoolName,
                                Description = edu.Description
                            };
                            eduExp.StartTime = ConvertToBsonDateTime(edu.StartDate);
                            eduExp.EndTime = ConvertToBsonDateTime(edu.EndDate);
                            langProfile.EduExps.Add(eduExp);
                        }
                    }
                    if (content.Projects != null && content.Projects.ProjectMpr != null && content.Projects.ProjectMpr.Projects != null)
                    {
                        foreach (var project in content.Projects.ProjectMpr.Projects)
                        {
                            var projectExp = new ProjectExp()
                            {
                                Description = project.Description,
                                Name = project.Title
                            };

                            projectExp.StartTime = ConvertToBsonDateTime(project.StartDate);
                            projectExp.EndTime = ConvertToBsonDateTime(project.EndDate);
                            langProfile.ProjectExps.Add(projectExp);
                        }
                    }
                }

                model.AutoId = GetAutoId();
                MongoCollection.Insert(model);
                Console.WriteLine("写入数据成功,用户名:{0} {1}, AutoId: {2} ", model.LangProfiles.EN.LastName, model.LangProfiles.EN.FirstName, model.AutoId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("写入数据失败， " + ex.Message + "stack: " + ex.StackTrace);
                insertSucceed = false;
                throw;
            }

            return insertSucceed;
        }

        public static void Remove()
        {
            MongoCollection.RemoveAll();
        }

        public static BsonDateTime ConvertToBsonDateTime(string dateTime)
        {
            BsonDateTime result = null;
            if (!string.IsNullOrEmpty(dateTime))
            {
                DateTime time;
                int value;
                dateTime = dateTime.Replace("&dsh;", "-");
                if (Int32.TryParse(dateTime, out value))
                {
                    dateTime = string.Format("{0}-01-01", dateTime);
                }
                if (DateTime.TryParse(dateTime, out time))
                {
                    result = BsonDateTime.Create(dateTime);
                }

            }

            return result;
        }

    }
}
        