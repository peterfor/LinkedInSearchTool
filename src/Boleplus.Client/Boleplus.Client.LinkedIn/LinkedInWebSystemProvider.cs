using BolePlus.Client.Configuration;
using BolePlus.Client.Configuration.Configuration;
using BolePlus.Client.DataLayer.DataModel;
using BolePlus.Client.LinkedIn.DataAccessor;
using BolePlus.Client.LinkedIn.Json;
using BolePlus.Client.Web;
using BolePlus.Client.Web.Interop;
using BolePlus.Client.Web.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Concurrent;
using System.Threading;

namespace BolePlus.Client.LinkedIn
{
    public class LinkedInWebSystemProvider : IWebSystemProvider, IProfileProvider
    {
        public LinkedInWebSystemProvider()
        {
            this.LinkedInWebSystemConfig = new LinkedInWebConfig();
            this.PostalCodeConfiguration = ProviderConfiguration.PostalCodeConfiguration;
            this.ProxyConfiguration = ProviderConfiguration.ProxyConfiguration;
            this.FailedProfileConfiguration = new ProfileConfiguration()
            {
                ProfileCollection = new List<ProfileItem>()
            };
        }

        public PostalCodeConfiguration PostalCodeConfiguration { get; private set; }

        public ProxyConfiguration ProxyConfiguration { get; set; }

        public string ProviderId
        {
            get { return "LinkedIn"; }
        }

        public string DisplayName
        {
            get { return "LinkedIn"; }
        }

        public ProfileConfiguration FailedProfileConfiguration { get; set; }

        public LinkedInWebConfig LinkedInWebSystemConfig
        {
            get;
            private set;
        }

        public RequestSession CurrentSession { get; private set; }

        public bool Login(bool saveSession, out string message)
        {

            Console.WriteLine("登出LinkedIn...");
            this.CurrentSession = new RequestSession(this);
            string result;
            message = "";

            HttpRequestContext requestContext = new HttpRequestContext(this.LinkedInWebSystemConfig.MainEntryUrl)
            {
                Referer = LinkedInWebSystemConfig.MainEntryUrl,
                KeepAlive = true,
                TimeOut = 50000
            };
            HttpRequestUtils.RequestHtmlPage(requestContext, out result);
            CurrentSession.CookieContainer = requestContext.CookieContainer;

            result = string.Empty;
            HttpRequestContext loginRequestContext = new HttpRequestContext(this.LinkedInWebSystemConfig.LoginUrl)
            {
                Referer = LinkedInWebSystemConfig.MainEntryUrl,
                KeepAlive = true,
                TimeOut = 50000
            };
            HttpRequestUtils.RequestHtmlPage(loginRequestContext, out result);
            CurrentSession.CookieContainer = loginRequestContext.CookieContainer;

            string csrfToken = string.Empty;
            string sourceAlias = string.Empty;

            csrfToken = this.GetCSRFToken(result);
            sourceAlias = this.GetSourceAlias(result);

            // string viewStateValue = string.Empty;
            //viewStateValue = this.GetViewState(result);

            Console.WriteLine("登陆LinkedIn...");
            string postData = String.Format(this.LinkedInWebSystemConfig.LoginPostDataFormat,
                                            HttpUtility.UrlEncode(this.LinkedInWebSystemConfig.LoginParameters["UserName"].Value),
                                            HttpUtility.UrlEncode(this.LinkedInWebSystemConfig.LoginParameters["Password"].Value),
                                            csrfToken,
                                            sourceAlias);
            HttpPostRequestContext postRequestContext = new HttpPostRequestContext(this.LinkedInWebSystemConfig.LoginSubmitUrl, postData)
            {
                CookieContainer = CurrentSession.CookieContainer,
                Referer = LinkedInWebSystemConfig.MainEntryUrl,
                KeepAlive = true,
                TimeOut = 50000
            };

            string loginResult;

            HttpRequestUtils.RequestPostHtmlPage(postRequestContext, out loginResult);

            return true;
        }

        public bool Login()
        {
            bool success = false;
            string message = "";
            int tryCount = 1;
            while (tryCount++ <= 2)
            {
                try
                {
                    success = Login(false, out message);
                    break;
                }
                catch (Exception ex)
                {
                    success = false;
                    Console.WriteLine("Login 失败,Error:{0}", ex.Message);
                }
            }

            return success;
        }

        public RequestSession Login(WebProxy proxy = null)
        {
            string address = proxy != null ? proxy.Address.ToString() : "";
            Console.WriteLine("登出LinkedIn...Proxy: {0}", address);
            var session = new RequestSession(this);
            string result;

            HttpRequestContext requestContext = new HttpRequestContext(this.LinkedInWebSystemConfig.MainEntryUrl)
            {
                Referer = LinkedInWebSystemConfig.MainEntryUrl,
                KeepAlive = true,
                TimeOut = 10000
            };
            HttpRequestUtils.RequestHtmlPage(requestContext, out result, proxy);
            session.CookieContainer = requestContext.CookieContainer;

            result = string.Empty;
            HttpRequestContext loginRequestContext = new HttpRequestContext(this.LinkedInWebSystemConfig.LoginUrl)
            {
                Referer = LinkedInWebSystemConfig.MainEntryUrl,
                KeepAlive = true,
                TimeOut = 10000
            };
            HttpRequestUtils.RequestHtmlPage(loginRequestContext, out result, proxy);
            session.CookieContainer = loginRequestContext.CookieContainer;

            string csrfToken = string.Empty;
            string sourceAlias = string.Empty;

            csrfToken = this.GetCSRFToken(result);
            sourceAlias = this.GetSourceAlias(result);

            // string viewStateValue = string.Empty;
            //viewStateValue = this.GetViewState(result);

            Console.WriteLine("登录LinkedIn...Proxy: {0}", address);
            string postData = String.Format(this.LinkedInWebSystemConfig.LoginPostDataFormat,
                                            HttpUtility.UrlEncode(this.LinkedInWebSystemConfig.LoginParameters["UserName"].Value),
                                            HttpUtility.UrlEncode(this.LinkedInWebSystemConfig.LoginParameters["Password"].Value),
                                            csrfToken,
                                            sourceAlias);
            HttpPostRequestContext postRequestContext = new HttpPostRequestContext(this.LinkedInWebSystemConfig.LoginSubmitUrl, postData)
            {
                CookieContainer = session.CookieContainer,
                Referer = LinkedInWebSystemConfig.MainEntryUrl,
                KeepAlive = true,
                TimeOut = 2000
            };

            string loginResult;

            HttpRequestUtils.RequestPostHtmlPage(postRequestContext, out loginResult, proxy);
            Console.WriteLine("登录LinkedIn成功....Proxy: {0}", address);
            session.CookieContainer = postRequestContext.CookieContainer;
            return session;
        }

        private string GetSourceAlias(string pageContent)
        {
            var sq = new SharpQuery(pageContent);
            var xe = sq.Find("input#sourceAlias-login");
            return xe.FirstOrDefault().Attributes["value"].InnerText;
        }

        private string GetCSRFToken(string pageContent)
        {
            var sq = new SharpQuery(pageContent);
            var xe = sq.Find("input#csrfToken-login");
            return xe.FirstOrDefault().Attributes["value"].InnerText;
        }

        public IDictionary<string, string> GetSiteInfo()
        {
            throw new NotImplementedException();
        }

        private WebProxy CreateProxyInstance(int index, out RequestSession session)
        {
            if (this.ProxyConfiguration.ProxyCollection.Count <= index)
            {
                index = 0;
            }

            var currentProxyConfig = this.ProxyConfiguration.ProxyCollection[index];
            WebProxy proxy = new WebProxy(currentProxyConfig.Ip, Convert.ToInt32(currentProxyConfig.Port));
            if (currentProxyConfig.Session == null)
            {
                currentProxyConfig.Session = Login(proxy);
            }

            session = currentProxyConfig.Session;
            Console.WriteLine();
            return proxy;
        }

        private IWebProxy CreateProxyInstance(ProxyItem proxyItem)
        {
            IWebProxy proxy = new WebProxy(proxyItem.Ip, Convert.ToInt32(proxyItem.Port));
            return proxy;
        }

        private void InitialCurrentSession(IWebProxy proxy = null)
        {
            string address = proxy != null ? ((WebProxy)proxy).Address.ToString() : "";

            try
            {
                Console.WriteLine("初始化Current Session: 代理:{0}", address);

                LinkedInWebConfig webConfig = (LinkedInWebConfig)this.LinkedInWebSystemConfig;
                string result = "";
                HttpRequestContext requestContext = new HttpRequestContext(webConfig.MainEntryUrl)
                {
                    Referer = LinkedInWebSystemConfig.MainEntryUrl,
                    KeepAlive = true,
                    TimeOut = 2000
                };
                HttpRequestUtils.RequestHtmlPage(requestContext, out result, proxy);
                CurrentSession.CookieContainer = requestContext.CookieContainer;

                Console.WriteLine("初始化Current Session成功,代理: {0}", address);
            }
            catch (Exception ex)
            {

                Console.WriteLine("初始化Current Session失败，Proxy: {1},Error: {0}", ex.Message, address);
                throw;
            }
        }

        public void GetProfileIds()
        {
            if (!Login())
            {
                return;
            }
           

            var configPath = this.GetConfigurationPath("ProfileIdSearch");
            var postalCodeConfig = ProviderConfiguration.GetConfigurationEntity<PostalCodeConfiguration>(configPath);
            var storedDirPath = string.Format(@"{0}\{1}", Environment.CurrentDirectory, "LinkedInSearchResult");
            if (!Directory.Exists(storedDirPath))
            {
                Directory.CreateDirectory(storedDirPath);
            }

            #region search code

            int totalSearchCount = 1;
            int cityCount = 0;
            int errorCount = 0;
            //   Parallel.ForEach(this.PostalCodeConfiguration.StateCollection, (state,loopState) =>
            Array.ForEach(postalCodeConfig.StateCollection.ToArray(), (state) =>
            {
                //if (loopState.IsStopped)
                //{
                //    loopState.Stop();
                //    return;
                //}

                //  Parallel.ForEach(state.PostalCodeCollection, (city,loopStateCity) =>
                Array.ForEach(state.PostalCodeCollection.ToArray(), (city) =>
                {
                   
                  
                    if (city.Checked)
                    {
                        return;
                    }
                    if (cityCount >= this.LinkedInWebSystemConfig.LoginConfiguration.PostCodeCount)
                    {
                        return;
                    }
                    cityCount++;
                    //if (loopStateCity.IsStopped)
                    //{
                    //    loopStateCity.Stop();
                    //    return;
                    //}
                    if (totalSearchCount >= 1300 || errorCount >= 10)
                    {
                        //   loopStateCity.Stop();
                        return;
                    }

                    #region Search Advanced Initialization
                    /*

                    string orig = "";
                    string rsid = "";

                    int tryCount = 1;
                    tryCount = 1;
                    while (tryCount++ <= 2)
                    {
                        try
                        {
                            Console.WriteLine("LinkedIn Advance Search Initailizaition , City:{0}, PostalCode: {1}...", city.Name, city.Code);
                            HttpRequestContext requestContext = new HttpRequestContext(this.LinkedInWebSystemConfig.AdvanceProfileUrl)
                            {
                                CookieContainer = this.CurrentSession.CookieContainer,
                                Referer = LinkedInWebSystemConfig.MainEntryUrl,
                                KeepAlive = true,
                                TimeOut = 15000
                            };
                            HttpRequestUtils.RequestHtmlPage(requestContext, out result);
                            orig = GetOrIg(result);
                            rsid = GetRsId(result);
                          //  Thread.Sleep(30000);
                            break;
                        }
                        catch (WebException ex)
                        {
                            errorCount++;
                            Console.WriteLine("获取数据失败,Error：{0}", ex.Message);
                            if (ex.Response != null)
                            {
                                var response = ex.Response as HttpWebResponse;
                                if (response != null)
                                {
                                    if (response.StatusCode == HttpStatusCode.Found)
                                    {
                                        errorCount = 50;
                                    }
                                }
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            Console.WriteLine("LinkedIn Advance Search Initailizaition , City:{0}, PostalCode: {1}, Error:{2}", city.Name, city.Code, ex.Message);
                        }
                    }
                      */

                    #endregion
                    int searchIndex = 1;
                    int webErrorCount = 0;
                    ErrorType errorType = ErrorType.None;

                    #region main search code
                    while (searchIndex <= 70)
                    {
                        totalSearchCount++;
                        if (errorType == ErrorType.NonResult || errorType == ErrorType.NotFullResult)
                        {
                            break;
                        }
                        int tryCount = 1;
                        while (tryCount++ <= 2)
                        {
                            try
                            {
                                string result = string.Empty;

                                Console.WriteLine("LinkedIn Advance Search Result , City:{0}, PostalCode: {1},页码:{2}...", city.Name, city.Code, searchIndex);
                                string searchPostData = string.Format(this.LinkedInWebSystemConfig.SearchPostDataFormat, city.Code, searchIndex);
                                HttpPostRequestContext requestContext = new HttpPostRequestContext(this.LinkedInWebSystemConfig.SearchProfileUrl, searchPostData)
                                {
                                    CookieContainer = this.CurrentSession.CookieContainer,
                                    Referer = LinkedInWebSystemConfig.MainEntryUrl,
                                    KeepAlive = true,
                                    TimeOut = 20000
                                };

                                HttpRequestUtils.RequestPostHtmlPage(requestContext, out result);
                                var fileName = city.Code + "_" + searchIndex;
                                errorType = LinkedInAnalyzer.CheckResponseForProfileId(result);
                                Thread.Sleep(5000);
                                if (errorType == ErrorType.NonResult)
                                {
                                    break;
                                }

                                File.WriteAllText(string.Format(@"{0}\{1}\{2}.txt", Environment.CurrentDirectory, "LinkedInSearchResult", fileName), result);
                                break;
                            }
                            catch (WebException ex)
                            {
                                errorCount++;
                                webErrorCount++;
                                Console.WriteLine("获取数据失败,Error：{0}", ex.Message);
                                if (ex.Response != null)
                                {
                                    var response = ex.Response as HttpWebResponse;
                                    if (response != null)
                                    {
                                        if (response.StatusCode == HttpStatusCode.Found)
                                        {
                                            errorType = ErrorType.NonResult;
                                            totalSearchCount = 1000;
                                            webErrorCount = 10;
                                        }
                                    }
                                }

                                break;
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                Console.WriteLine("LinkedIn Advance Search Result Exception , City:{0}, PostalCode: {1}, Error:{2},页码:{2}...", city.Name, city.Code, ex.Message, searchIndex);
                            }
                        }

                        searchIndex++;
                    }

                    #endregion

                    if (webErrorCount < 10)
                    {
                        city.Checked = true;
                    }
                });
            });

            #endregion

            int tr2yCount = 0;
            while (tr2yCount++ <= 2)
            {
                try
                {
                    ProviderConfiguration.SerializeConfigurationEntityToXml<PostalCodeConfiguration>(postalCodeConfig, configPath,true);
                    break;
                }
                catch (Exception ex)
                {

                }
            }

            Console.WriteLine("搜索结束了。。。。");
        }

        private string GetConfigurationPath(string directoryPath)
        {
            directoryPath = string.Format(@"{0}\{1}\{2}", Environment.CurrentDirectory, "config", directoryPath);
            if (Directory.Exists(directoryPath))
            {
                var files = Directory.GetFiles(directoryPath);
                if (files != null && files.Length > 0)
                {
                    return files.First();
                }
            }

            return "";
        }

        public void ProfileIdFileHandler()
        {
            LinkedInAnalyzer.ProfileConfigurationFromCollege = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromCollege);
            var collegeConfiguration = LinkedInAnalyzer.ProfileConfigurationFromCollege;

            var path = string.Format(@"{0}\{1}", Environment.CurrentDirectory, "LinkedInSearchResult");
            var totalProfilePath = string.Format(@"{0}\{1}", Environment.CurrentDirectory, "TotalLinkedInSearchResult");

            #region main code

            if (Directory.Exists(path))
            {
                var profiles = Directory.GetFiles(path);
                if (profiles != null)
                {
                    foreach (var file in profiles)
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            var content = File.ReadAllText(file);
                            bool success = false;

                            success = LinkedInAnalyzer.AnalysisProfileIdFromAdvancedSearch(content, collegeConfiguration);

                            if (!success)
                            {
                                Console.WriteLine("数据解析失败,path:{0}", file);
                            }
                            else
                            {
                                File.Move(fileInfo.FullName, string.Format(@"{0}\{1}", totalProfilePath, fileInfo.Name));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("数据解析出现异常,Error:{0}", ex.Message);
                        }
                    }
                }
            }

            #endregion
            try
            {
                int try2Count = 1;
                while (try2Count++ <= 2)
                {
                    LinkedInAnalyzer.SaveProfileFromCollege();
                    break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void SplitPostalCode()
        {
            foreach (var state in this.PostalCodeConfiguration.StateCollection)
            {
                var postalCodeConfig = new PostalCodeConfiguration()
                {
                    StateCollection = new List<StateItem>()
                     {
                         state
                     }
                };

                try
                {
                    ProviderConfiguration.SerializeConfigurationEntityToXml<PostalCodeConfiguration>(postalCodeConfig, state.Code);
                }
                catch (Exception ex)
                {

                }
            }

            Console.WriteLine("拆分操作成功!");
        }

        public void MergePostalFromDifferentState()
        {
            this.PostalCodeConfiguration = ProviderConfiguration.GetConfigurationEntity<PostalCodeConfiguration>(ConfigurationType.USPostalCode);
            var path = string.Format(@"{0}\{1}", Environment.CurrentDirectory, "config\\profileId");
            if (Directory.Exists(path))
            {
                var profiles = Directory.GetFiles(path);
                if (profiles != null)
                {
                    foreach (var file in profiles)
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            var stateConfig = ProviderConfiguration.GetConfigurationEntity<PostalCodeConfiguration>(file);
                            if (!stateConfig.StateCollection.First().PostalCodeCollection.Any(i => i.Checked == false))
                            {
                                if (!fileInfo.Name.Contains("_Checked"))
                                {
                                    var desFileName = fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.'));
                                    var destinationPath = string.Format(@"{0}\{1}\{2}", Environment.CurrentDirectory, "config\\profileId", desFileName + "_Checked.xml");
                                    File.Move(fileInfo.FullName, destinationPath);
                                }
                            }
                            if (this.PostalCodeConfiguration.StateDic.ContainsKey(stateConfig.StateCollection.First().Code))
                            {
                                this.PostalCodeConfiguration.StateDic[stateConfig.StateCollection.First().Code].PostalCodeCollection = stateConfig.StateCollection.First().PostalCodeCollection;
                                Console.WriteLine("整合{0} Postal Code成功!", fileInfo.Name);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }

            int tr2yCount = 0;
            while (tr2yCount++ <= 2)
            {
                try
                {
                    this.SavePostalCode();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            Console.WriteLine("整合PostalCode完成....");
        }

        private string GetRsId(string pageContent)
        {
            SharpQuery sq = new SharpQuery(pageContent);

            //  new Regex(@"(?<=<td>)(.*?)(?=</td>)"
            Regex reg = new Regex(@"rsid=[0-9]*?&");
            var resultString = reg.Match(pageContent).Value;

            int start = resultString.IndexOf('=');
            int end = resultString.IndexOf('&');

            return resultString.Substring(start + 1, end - start - 1);
        }

        private string GetOrIg(string pageContent)
        {

            Regex reg = new Regex("<input name=\"orig\" type=\"hidden\" [\\s\\S]*?>");
            var resultString = reg.Match(pageContent).Value;

            int start = resultString.LastIndexOf('=');
            int end = resultString.LastIndexOf('"');

            return resultString.Substring(start + 2, end - start - 2);
        }

        public void GetProfiles()
        {
            var searchProfileConfig = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(this.GetConfigurationPath("ProfileView"));
            var storedDirPath = string.Format(@"{0}\{1}", Environment.CurrentDirectory, "LinkedIn");
            if (!Directory.Exists(storedDirPath))
            {
                Directory.CreateDirectory(storedDirPath);
            }
            if (!Login())
            {
                return;
            }

            int errorCount = 1;
            int searchTotalCount = 0;
            Parallel.ForEach(searchProfileConfig.ProfileCollection, (profile, loopState) =>
            {
                if (loopState.IsStopped)
                {
                    loopState.Stop();
                    return;
                }
                if (errorCount >= 20 || searchTotalCount >= 700)
                {
                    loopState.Stop();
                    return;
                }
                if (profile.HasViewed)
                {
                    return;
                }
                Thread.Sleep(30000);
                int tryCount = 1;
                if (string.Empty.Equals(profile.ViewProfileUrl))
                {
                    return;
                }

                profile.ViewProfileUrl = profile.ViewProfileUrl.Replace("&amp;", "&");

                string result = string.Empty;

                while (tryCount++ <= 2)
                {
                    var erroType = ErrorType.None;
                    try
                    {
                        searchTotalCount++;
                        Console.WriteLine("LinkedIn 搜索人才  Id:{0},尝试次数: {1}", profile.Id, tryCount - 1);

                        HttpRequestContext requestContext = new HttpRequestContext(profile.ViewProfileUrl)
                        {
                            CookieContainer = this.CurrentSession.CookieContainer,
                            Referer = LinkedInWebSystemConfig.MainEntryUrl,
                            KeepAlive = true,
                            TimeOut = 60000
                        };
                        HttpRequestUtils.RequestHtmlPage(requestContext, out result);
                        File.WriteAllText(string.Format(@"{0}\{1}\{2}.txt", Environment.CurrentDirectory, "LinkedIn", profile.Id), result);
                        Thread.Sleep(60000);
                        break;
                    }
                    catch (WebException ex)
                    {
                        //    errorCount++;
                        Console.WriteLine("LinkedIn 搜索人才  Id:{0},尝试次数: {1}", profile.Id, tryCount);
                        if (ex.Response != null)
                        {
                            var response = ex.Response as HttpWebResponse;
                            if (response != null)
                            {
                                if (response.StatusCode == HttpStatusCode.Found)
                                {
                                    //  errorCount = 50;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // errorCount++;
                        Console.WriteLine("LinkedIn 搜索人才  Id:{0},尝试次数: {1}", profile.Id, tryCount);

                    }
                    finally
                    {
                        if (tryCount > 2)
                        {
                            // errorCount++;
                            Console.WriteLine("LinkedIn 搜索人才失败，Id:{0}", profile.Id);
                        }
                    }
                    break;
                }

            });

        }

        public void ProfileFileHandler()
        {
            LinkedInAnalyzer.ProfileConfigurationFromCollege = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromCollege);


            var collegeConfiguration = LinkedInAnalyzer.ProfileConfigurationFromCollege; // ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromCollege);

            //var collegeConfiguration = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromCollege);
            var path = string.Format(@"{0}\{1}", Environment.CurrentDirectory, "LinkedIn");
            var totalProfilePath = string.Format(@"{0}\{1}", Environment.CurrentDirectory, "TotalProfiles");

            #region merge Code

            //if (Directory.Exists(totalProfilePath))
            //{
            //    var profiles = Directory.GetFiles(totalProfilePath);
            //    if (profiles != null)
            //    {
            //        foreach (var file in profiles)
            //        {
            //            FileInfo fileInfo = new FileInfo(file);
            //            int id = Convert.ToInt32(fileInfo.Name.Substring(0, fileInfo.Name.IndexOf('.')));
            //            if (collegeConfiguration.ItemDic.ContainsKey(id))
            //            {
            //                collegeConfiguration.ItemDic[id].HasViewed = true;
            //            }
            //        }
            //    }
            //}

            #endregion

            #region main code

            if (Directory.Exists(path))
            {
                var profiles = Directory.GetFiles(path);
                if (profiles != null)
                {
                    foreach (var file in profiles)
                    {
                        try
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            bool success = false;
                            var content = File.ReadAllText(file);
                            var profileModel = LinkedInAnalyzer.AnaylysisProfile(content);
                            if (profileModel != null && profileModel.Content != null && profileModel.Content.GobalInfo != null && profileModel.Content.GobalInfo.Id != 0)
                            {
                                if (collegeConfiguration.ItemDic.ContainsKey(profileModel.Content.GobalInfo.Id))
                                {
                                    if (collegeConfiguration.ItemDic[profileModel.Content.GobalInfo.Id].HasViewed == true)
                                    {
                                        Console.WriteLine("数据重复了,舍弃ProfileId：{0}", profileModel.Content.GobalInfo.Id);
                                        continue;
                                    }
                                    success = ProfileDataAccessor.Insert(profileModel);
                                    if (success)
                                    {
                                        collegeConfiguration.ItemDic[profileModel.Content.GobalInfo.Id].HasViewed = true;
                                    }
                                }
                                else
                                {
                                    success = ProfileDataAccessor.Insert(profileModel);
                                }
                            }

                            if (!success)
                            {
                                Console.WriteLine("数据解析失败,path:{0}", path);
                            }
                            else
                            {
                                File.Move(fileInfo.FullName, string.Format(@"{0}\{1}", totalProfilePath, fileInfo.Name));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("数据解析出现异常,Error:{0}", ex.Message);
                        }
                    }
                }
            }

            #endregion
            try
            {
                int try2Count = 1;
                while (try2Count++ <= 2)
                {
                    LinkedInAnalyzer.SaveProfileFromCollege();
                    break;
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void SavePostalCode()
        {
            try
            {
                ProviderConfiguration.SerializeConfigurationEntityToXml<PostalCodeConfiguration>(ConfigurationType.USPostalCode, this.PostalCodeConfiguration);
            }
            catch (Exception ex)
            {

            }
        }

        private HttpRequestContext BuildHttpRequestContext(string postalCode, string rsid, string orid, RequestSession session)
        {
            string searchProfileUrl = String.Format(this.LinkedInWebSystemConfig.SearchProfileUrl,
                                                   System.Web.HttpUtility.UrlEncode(postalCode),
                                                   rsid,
                                                   orid);
            HttpRequestContext searchProfleRequestContext = new HttpRequestContext(searchProfileUrl)
            {
                CookieContainer = session.CookieContainer,
                Referer = LinkedInWebSystemConfig.MainEntryUrl,
                KeepAlive = true,
                TimeOut = 15000
            };

            return searchProfleRequestContext;
        }

        IWebSystemConfig IWebSystemProvider.WebSystemConfig
        {
            get { throw new NotImplementedException(); }
        }

        public void GetProfilesTestForJson()
        {
            var path = @"D:\重要文件\src\src\Boleplus.Client\BolePlus.Client.LinkedIn\Schema\{0}.txt";


            for (int i = 5; i >= 1; i--)
            {
                var value = File.ReadAllText(string.Format(path, i));
                string contentString = value;
                contentString = contentString.Replace("&dsh;", "-");
                contentString = contentString.Replace("&amp;#x2022;", "•");
                contentString = contentString.Replace("&amp;quot;", "\'");


                var searchResult = LinkedInAnalyzer.DeserializeContent<LinkedInProfile>(contentString);

                ProfileDataAccessor.Insert(searchResult);
            }
        }

        List<Profile> IProfileProvider.GetProfiles()
        {
            throw new NotImplementedException();
        }

        public void CheckProfileStatus()
        {
            #region <<查看有效简历>>

            var profileConfig = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromCollege);
            ConcurrentBag<ProfileItem> validProfileCollection = new ConcurrentBag<ProfileItem>();
            ConcurrentDictionary<int, ProfileItem> validProfileDicCollection = new ConcurrentDictionary<int, ProfileItem>();
            Parallel.ForEach(profileConfig.ProfileCollection, (profile) =>
            {
                if (!validProfileDicCollection.ContainsKey(profile.Id))
                {
                    validProfileDicCollection[profile.Id] = profile;
                    if (!profile.ViewProfileUrl.Contains("OUT_OF_NETWORK"))
                    {
                        validProfileCollection.Add(profile);
                    }
                }
            });

            Console.WriteLine("有效简历:{0}", validProfileDicCollection.Count);
            Console.WriteLine("有Full Name简历:{0}", validProfileCollection.Count);
            #endregion
        }

        public void GetProfileIdsFromCollege()
        {
            string searchBasehUrl = "http://www.linkedin.com/college/peers?start={0}&count={1}&edu-school=11398&edu-school-start={2}&edu-school-end={2}&edu-grad={3}&incl-users-with-no-edu-dates=true&hideconn=false&facet[]=location,us:0 ";

            string baseUrl = "http://www.linkedin.com/college/peers?start=450&count=50&edu-school=11398&edu-school-start=1939&edu-school-end=2012&edu-start=1939&edu-end=2020&incl-users-with-no-edu-dates=false&hideconn=false&facet[]=location,us:0";


            if (!Login())
            {
                return;
            }



            int count = 1940;
            List<int> queryList = new List<int>();
            Dictionary<int, int> queryDic = new Dictionary<int, int>();

            while (count <= 2020)
            {
                queryList.Add(count);
                count += 1;
            }

            Parallel.ForEach(queryList, (query) =>
            {
                int queryTotalCount = 0;
                int start = 0;
                var errorType = ErrorType.None;
                while (errorType != ErrorType.NonResult)
                {
                    try
                    {
                        int eachCount = 0;
                        string result = "";
                        Console.WriteLine("LinkedIn College Search Initailizaition ,Year:{0},Start:{1}", query, start);
                        var searchUrl = string.Format(searchBasehUrl, start, 12, query,query);
                        start += 12;
                        HttpRequestContext requestContext = new HttpRequestContext(searchUrl)
                        {
                            CookieContainer = this.CurrentSession.CookieContainer,
                            Referer = LinkedInWebSystemConfig.MainEntryUrl,
                            KeepAlive = true,
                            TimeOut = 15000
                        };
                        HttpRequestUtils.RequestHtmlPage(requestContext, out result);
                        errorType = LinkedInAnalyzer.AnalysisCollegeProfile(result, out eachCount);
                        queryTotalCount += eachCount;
                      //  Thread.Sleep(50000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("LinkedIn College Search Initailizaition Exception, Error:{0}", ex.Message);
                    }
                }
                Console.WriteLine("Year:{0}, TotalCount:{1}", query, queryTotalCount);
                queryDic.Add(query, queryTotalCount);
            });

            LinkedInAnalyzer.SaveProfileFromCollege();
            ProviderConfiguration.SerializeConfigurationEntityToXml<ProfileConfiguration>(ConfigurationType.ProfileFromCollege, LinkedInAnalyzer.ProfileConfigurationFromCollege);
            string path = string.Format(string.Format(@"{0}\{1}\{2}", Environment.CurrentDirectory, "config", "queryResult.text"));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            StreamWriter writer = new StreamWriter(path);
            var keyList = queryDic.Keys.ToList();
            keyList.Sort();
            foreach (var key in keyList)
            {
                writer.WriteLine(string.Format("{0}  :  {1}", key, queryDic[key]));
            }
            writer.Flush();
            writer.Close();

            CheckProfileStatus();
        }

        public void GetProfileFromPeopleYouMayKnow()
        {
            //string name2 = "";
            //var dd = LinkedInAnalyzer.AnalysisProfileFromPeopleYouMayKnow(File.ReadAllText(@"D:\重要文件\src\src\Boleplus.Client\BolePlus.Client.LinkedIn\1.txt"), ref name2);
            if (!Login())
            {
                return;
            }

            var searchBaseUrl = "http://www.linkedin.com/pymk/pcard?mid={0}";

            LinkedInAnalyzer.ProfileConfigurationFromPeopleYouMayKnow = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromCollege);

            if (LinkedInAnalyzer.ProfileConfigurationFromPeopleYouMayKnow != null && LinkedInAnalyzer.ProfileConfigurationFromPeopleYouMayKnow.ProfileCollection != null)
            {
                int searchTotalCount = 0;
                int errorCount = 1;
                ErrorType type = ErrorType.None;
                Parallel.ForEach(LinkedInAnalyzer.ProfileConfigurationFromPeopleYouMayKnow.ProfileCollection, (profile, loopState) =>
                {

                    if (loopState.IsStopped)
                    {
                        loopState.Stop();
                        return;
                    }
                    if (searchTotalCount >= 700)
                    {
                        loopState.Stop();
                        return;
                    }
                    searchTotalCount++;
                    try
                    {
                        if (type == ErrorType.Capture || errorCount >= 20 || searchTotalCount >= 700)
                        {
                            Console.WriteLine("获取数据被抓，赶快终止!!!!!!");

                            loopState.Stop();
                            return;
                        }
                        if (profile.Checked)
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                        return;
                    }
                    Thread.Sleep(20000);
                    int tryCount = 1;
                    while (tryCount++ <= 2)
                    {
                        try
                        {
                           
                            string result;
                            var searchUrl = string.Format(searchBaseUrl, profile.Id);
                            HttpRequestContext requestContext = new HttpRequestContext(searchUrl)
                            {
                                CookieContainer = this.CurrentSession.CookieContainer,
                                Referer = LinkedInWebSystemConfig.MainEntryUrl,
                                KeepAlive = true,
                                TimeOut = 50000
                            };
                            HttpRequestUtils.RequestHtmlPage(requestContext, out result);
                            string name = "";
                            var viewProfileUrl = LinkedInAnalyzer.AnalysisProfileFromPeopleYouMayKnow(result, ref name);
                            if (string.IsNullOrEmpty(viewProfileUrl))
                            {
                                continue;
                            }
                            profile.Checked = true;
                            profile.Name = name;
                            profile.ViewProfileUrl = viewProfileUrl;
                            Console.WriteLine("获取人才数据,ProfileId：{0}，ProfileName：{1}", profile.Id, profile.Name);
                            Thread.Sleep(40000);
                        }
                        catch (WebException ex)
                        {
                            errorCount++;
                            Console.WriteLine("获取数据失败,ProfileId：{0}，Error：{1}", profile.Id, ex.Message);
                            if (ex.Response != null)
                            {
                                var response = ex.Response as HttpWebResponse;
                                if (response != null)
                                {
                                    if (response.StatusCode == HttpStatusCode.Found)
                                    {
                                        type = ErrorType.Capture;
                                    }
                                }
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("获取数据失败,ProfileId：{0}，Error：{1}", profile.Id, ex.Message);
                            break;
                        }

                        break;
                    }

                });

                int tr2yCount = 1;
                while (tr2yCount++ <= 2)
                {
                    try
                    {
                        LinkedInAnalyzer.SaveProfileFromPeopleYouMayKnow();
                        break;
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
        }

        public void MergeProfileFromPeopleYouMayKnowToCollege()
        {
            var result = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromPeopleYouMayKnowResult);
            LinkedInAnalyzer.ProfileConfigurationFromCollege = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromCollege);
            int count = 0;
            Parallel.ForEach(result.ProfileCollection, (profile, loopState) =>
            {
                if (profile.ViewProfileUrl!=null &&!profile.ViewProfileUrl.Contains("OUT_OF_NETWORK"))
                {
                    if (LinkedInAnalyzer.ProfileConfigurationFromCollege.ItemDic.ContainsKey(profile.Id))
                    {
                        count++;
                        LinkedInAnalyzer.ProfileConfigurationFromCollege.ItemDic[profile.Id].Checked = true;
                        LinkedInAnalyzer.ProfileConfigurationFromCollege.ItemDic[profile.Id].Name = profile.Name;
                        LinkedInAnalyzer.ProfileConfigurationFromCollege.ItemDic[profile.Id].ViewProfileUrl = profile.ViewProfileUrl;
                    }
                }
            });

            Console.WriteLine("总数:{0}", count);
            LinkedInAnalyzer.SaveProfileFromCollege();
        }

        public void MergeProfileFromCollegeToCollege()
        {
            var result = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromCollege);
            LinkedInAnalyzer.ProfileConfigurationFromCollege = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromCollege2);
            int count = 0;
            Array.ForEach(result.ProfileCollection.ToArray(), (profile) =>
            {
                try
                {
                    if (!LinkedInAnalyzer.ProfileConfigurationFromCollege.ItemDic.ContainsKey(profile.Id))
                    {
                        count++;
                        LinkedInAnalyzer.ProfileConfigurationFromCollege.ProfileCollection.Add(profile);
                        LinkedInAnalyzer.ProfileConfigurationFromCollege.ItemDic.Add(profile.Id, profile);
                    }
                }
                catch (Exception ex)
                {

                }
            });

            Console.WriteLine("总数:{0}", count);
            LinkedInAnalyzer.SaveProfileFromCollege();
        }

        public void SpliteProfileConfiguration()
        {
            Dictionary<int, List<ProfileItem>> splitedProfileDic = new Dictionary<int, List<ProfileItem>>();
            var collegeConfiguration = ProviderConfiguration.GetConfigurationEntity<ProfileConfiguration>(ConfigurationType.ProfileFromCollege);
            int index = 1;
            foreach (var profileItem in collegeConfiguration.ProfileCollection)
            {
                if (profileItem.HasViewed == true)
                {
                    continue;
                }
                if (splitedProfileDic.ContainsKey(index))
                {
                    if (splitedProfileDic[index].Count >= 700)
                    {
                        index++;
                    }
                    else
                    {
                        splitedProfileDic[index].Add(profileItem);
                    }
                }
                else
                {
                    splitedProfileDic.Add(index, new List<ProfileItem>() { profileItem });
                }
            }
            foreach (var key in splitedProfileDic.Keys)
            {
                var profileIdConfiguration = new ProfileConfiguration()
                {
                    ProfileCollection = new List<ProfileItem>()
                };
                profileIdConfiguration.ProfileCollection = splitedProfileDic[key];
                try
                {
                    ProviderConfiguration.SerializeConfigurationEntityToXml<ProfileConfiguration>(profileIdConfiguration, key.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}