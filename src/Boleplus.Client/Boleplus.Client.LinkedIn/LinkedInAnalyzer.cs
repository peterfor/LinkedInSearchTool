using BolePlus.Client.Configuration;
using BolePlus.Client.Web.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using BolePlus.Client.Configuration.Configuration;
using BolePlus.Client.LinkedIn.Json;
using BolePlus.Client.LinkedIn.DataAccessor;
using System.Web;

namespace BolePlus.Client.LinkedIn
{
    public class LinkedInAnalyzer
    {
        static LinkedInAnalyzer()
        {
            LinkedInAnalyzer.ProfileConfiguration = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.Profile);
            ProfileConfigurationFromCollege = new ProfileConfiguration()
            {
                ProfileCollection = new List<ProfileItem>()
            };
        }

        public LinkedInAnalyzer()
        {

        }

        public static string BaseUrl = "http://www.linkedin.com";

        public static ErrorType CheckResponseForProfileId(string pageContent)
        {
            ErrorType errorType = ErrorType.None;

            SharpQuery sq = new SharpQuery(pageContent);
            var result = sq.Find("a.trk-profile-name");

            if (result == null)
            {
                errorType = ErrorType.NonResult;
            }
            else
            {
                if (result.Count() == 0)
                {
                    errorType = ErrorType.NonResult;
                }
                else if (result.Count() < 10)
                {
                    errorType = ErrorType.NotFullResult;
                }
            }

            return errorType;
        }

        public static bool AnalysisProfileIdFromAdvancedSearch(string pageContent, ProfileConfiguration profileConfig)
        {
            bool success = false;
            SharpQuery sq = new SharpQuery(pageContent);
            var result = sq.Find("a.trk-profile-name");
            try
            {
                if (result != null && result.Count() != 0)
                {
                    success = true;
                    var resultEnumerator = result.GetEnumerator();
                    while (resultEnumerator.MoveNext())
                    {
                        var profileLink = resultEnumerator.Current;
                        if (profileLink.HasAttribute("href"))
                        {
                            var href = profileLink.Attributes["href"].Value;
                            href = href.Replace("&amp;", "&");
                            href = href.Substring(href.IndexOf('?') + 1);
                            int profileId = Convert.ToInt32(HttpUtility.ParseQueryString(href).Get("id"));
                            if (!profileConfig.ItemDic.ContainsKey(profileId))
                            {
                                Console.WriteLine("添加一条新的ProfileId：{0}", profileId);
                                var profileItem = new ProfileItem()
                                {
                                    Id = profileId
                                };
                                profileConfig.ProfileCollection.Add(profileItem);
                                profileConfig.ItemDic.Add(profileId, profileItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine(ex.Message);
            }

            return success;
        }

        public static T DeserializeContent<T>(string content)
        {
            T result = default(T);

            try
            {
                DataContractJsonSerializer serialzer = new DataContractJsonSerializer(typeof(T));

                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

                result = (T)serialzer.ReadObject(ms);
            }
            catch (Exception ex)
            {
                Console.WriteLine("反序列化失败。。。。。" + ex.Message);
            }

            return result;

        }

        public static ProfileConfiguration ProfileConfiguration { get; set; }

        public static ProfileConfiguration ProfileConfigurationFromCollege { get; set; }

        public static ProfileConfiguration ProfileConfigurationFromPeopleYouMayKnow { get; set; }

        public static int SearchCount { get; set; }

        public static bool StoreSearchResult(LinkedInSearchResult searchResult)
        {
            //if (searchResult == null || searchResult.Content == null || searchResult.Content.SearchJson == null || searchResult.Content.SearchJson.Search == null || searchResult.Content.SearchJson.Search.Results == null || searchResult.Content.SearchJson.Search.Results.Count == 0)
            //{
            //    return false;
            //}

            //foreach (var item in searchResult.Content.SearchJson.Search.Results)
            //{
            //    if (!LinkedInAnalyzer.ProfileConfiguration.ProfileIdItems.Contains(item.PersonDetail.Id))
            //    {
            //        ProfileItem profileItem = new ProfileItem()
            //        {
            //            Id = item.PersonDetail.Id,
            //            ViewProfileUrl = string.Format("{0}{1}", LinkedInAnalyzer.BaseUrl, item.PersonDetail.ViewProfileUrl)
            //        };

            //        LinkedInAnalyzer.ProfileConfiguration.AddProfileId(item.PersonDetail.Id);

            //        LinkedInAnalyzer.ProfileConfiguration.ProfileCollection.Add(profileItem);
            //    }
            //}

            //Console.WriteLine("存储次数成功",++LinkedInAnalyzer.SearchCount);
            return true;
        }

        public static void StoreResultToFile()
        {
            ProviderConfiguration.SerializeConfigurationEntityToXml<ProfileConfiguration>(ConfigurationType.Profile, LinkedInAnalyzer.ProfileConfiguration);
        }

        public static LinkedInProfile SerializeLinkedInProfile(IEnumerable<XmlElement> result)
        {
            var resultElement = (XmlElement)result.First();

            StringBuilder contentString = new StringBuilder(resultElement.InnerXml);
            contentString = contentString.Replace("<!--", "");
            contentString = contentString.Replace("-->", "");
            contentString = contentString.Replace(":&dsh;", ":");
            contentString = contentString.Replace("&dsh;", "-");
            contentString = contentString.Replace("&amp;#x2022;", "•");
            contentString = contentString.Replace("&amp;quot;", "\'");

            try
            {
                return LinkedInAnalyzer.DeserializeContent<LinkedInProfile>(contentString.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("序列化失败，Error：{0}", ex.Message);
            }

            return null;
        }

        public static LinkedInProfile AnaylysisProfile(string pageContent)
        {
            LinkedInProfile profile = null;
            LinkedInProfile headProfile = null;
            LinkedInProfile linkedInProfile = null;
            ErrorType errorType = ErrorType.None;
            SharpQuery sq = new SharpQuery(pageContent);

            var result = sq.Find("code#profile_v2_teamlink_v2-content");
            if (result != null && result.Count() != 0)
            {
                linkedInProfile = SerializeLinkedInProfile(result);
            }

            result = sq.Find("code#profile_v2_background-content");
            if (result != null && result.Count() != 0)
            {
                profile = SerializeLinkedInProfile(result);
            }

            result = sq.Find("code#top_card-content");

            if (result != null && result.Count() != 0)
            {
                headProfile = SerializeLinkedInProfile(result);
            }
            else
            {
                headProfile = new LinkedInProfile();
            }
            if (headProfile != null && profile != null)
            {
                headProfile.Content.Discovery = profile.Content.Discovery;
                headProfile.Content.Education = profile.Content.Education;
                headProfile.Content.Experience = profile.Content.Experience;
                headProfile.Content.Languages = profile.Content.Languages;
                headProfile.Content.Projects = profile.Content.Projects;
                headProfile.Content.Skills = profile.Content.Skills;
                headProfile.Content.Summary = profile.Content.Summary;
                headProfile.Content.GobalInfo = profile.Content.GobalInfo;
            }
            if (linkedInProfile != null && headProfile != null)
            {
                headProfile.Content.Discovery = linkedInProfile.Content.Discovery;
                headProfile.Content.GobalInfo = linkedInProfile.Content.GobalInfo;
            }

            return headProfile;
        }

        public static int CalculateTotalCount(string pageContent)
        {
            int totalCount = 0;

            CollegeProfile profile = AnalysisCollegeProfileModel(pageContent);
            if (profile != null && profile.CollecgeProfileDetail != null)
            {
                totalCount = profile.CollecgeProfileDetail.TotalCount;
            }

            return totalCount;
        }

        public static CollegeProfile AnalysisCollegeProfileModel(string pageContent)
        {
            var searchResult = LinkedInAnalyzer.DeserializeContent<CollegeProfile>(pageContent);
            return searchResult;
        }

        public static ErrorType AnalysisCollegeProfile(string pageContent, out int count)
        {
            count = 0;
            ErrorType type = ErrorType.None;
            CollegeProfile profile = AnalysisCollegeProfileModel(pageContent);
            if (profile != null && profile.CollecgeProfileDetail != null && profile.CollecgeProfileDetail.ProfileCollection != null)
            {
                if (profile.CollecgeProfileDetail.ProfileCollection.Count < 12)
                {
                    type = ErrorType.NonResult;
                }
                else
                {
                    foreach (var item in profile.CollecgeProfileDetail.ProfileCollection)
                    {
                        if (!LinkedInAnalyzer.ProfileConfigurationFromCollege.ProfileIdItems.Contains(item.Id))
                        {
                            count++;
                            ProfileItem profileItem = new ProfileItem()
                            {
                                Id = item.Id,
                                ViewProfileUrl = string.Format("{0}{1}", LinkedInAnalyzer.BaseUrl, item.ViewProfileUrl),
                                Name = item.Name
                            };

                            LinkedInAnalyzer.ProfileConfigurationFromCollege.AddProfileId(item.Id);

                            LinkedInAnalyzer.ProfileConfigurationFromCollege.ProfileCollection.Add(profileItem);
                        }
                    }

                    //  ProviderConfiguration.SerializeConfigurationEntityToXml<ProfileConfiguration>(ConfigurationType.ProfileFromCollege, LinkedInAnalyzer.ProfileConfigurationFromCollege);
                }
            }
            else
            {
                type = ErrorType.NonResult;
            }

            return type;
        }

        public static void SaveProfileFromCollege()
        {
            try
            {
                ProviderConfiguration.SerializeConfigurationEntityToXml<ProfileConfiguration>(ConfigurationType.ProfileFromCollege, LinkedInAnalyzer.ProfileConfigurationFromCollege);
            }
            catch (Exception ex)
            {

            }
        }

        public static void SaveProfileFromPeopleYouMayKnow()
        {
            try
            {
                ProviderConfiguration.SerializeConfigurationEntityToXml<ProfileConfiguration>(ConfigurationType.ProfileFromPeopleYouMayKnowResult, LinkedInAnalyzer.ProfileConfigurationFromPeopleYouMayKnow);
            }
            catch (Exception ex)
            {

            }
        }

        internal static string AnalysisProfileFromPeopleYouMayKnow(string pageContent, ref string name)
        {
            string viewProfileUrl = "";
            try
            {
                SharpQuery sq = new SharpQuery(pageContent);
                var result = sq.Find("a.fn");
                if (result != null && result.Count() != 0)
                {
                    XmlElement viewProfileLink = result.First();
                    if (viewProfileLink.HasAttribute("href"))
                    {
                        viewProfileUrl = string.Format("{0}{1}", BaseUrl, viewProfileLink.Attributes["href"].Value);
                    }
                }

                var namePhoto = sq.Find("img.photo");
                if (namePhoto != null && namePhoto.Count() != 0)
                {
                    XmlElement viewProfileLink = namePhoto.First();
                    if (viewProfileLink.HasAttribute("alt"))
                    {
                        name = viewProfileLink.Attributes["alt"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("解析错误,Error：{0}", ex.Message);
            }

            return viewProfileUrl;
        }
    }
}