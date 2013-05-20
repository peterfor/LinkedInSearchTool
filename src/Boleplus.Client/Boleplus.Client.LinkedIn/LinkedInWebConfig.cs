using BolePlus.Client.Configuration;
using BolePlus.Client.DataLayer.DataModel;
using BolePlus.Client.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BolePlus.Client.LinkedIn
{
    public class LinkedInWebConfig : IWebSystemConfig, IProfileConfig
    {
        public LinkedInWebConfig()
        {
           var dirPath = string.Format(string.Format(@"{0}\{1}\{2}", Environment.CurrentDirectory, "config","LoginConfig"));
           if (Directory.Exists(dirPath))
           {
               var files = Directory.GetFiles(dirPath);
               if (files != null && files.Length > 0)
               {
                   var configPath = files.First();
                   this.LoginConfiguration = ProviderConfiguration.GetConfigurationEntity<LoginConfiguration>(configPath);
               }
           }
        }


        public LoginConfiguration LoginConfiguration { get; set; }

        private IDictionary<string, LoginParameter> _loginParameters = null;

        public IDictionary<string, LoginParameter> LoginParameters
        {
            get
            {
                if (this._loginParameters == null)
                {
                    this._loginParameters = new Dictionary<string, LoginParameter>();
                    this._loginParameters.Add("UserName", new LoginParameter()
                    {
                        DisplayName = "用户名",
                        IsRequired = true,
                        RequireEncryption = false,
                      // Value = "peterlei@live.cn",
                             Value = this.LoginConfiguration.UserName,
                     //   Value="523182450@qq.com",
                       //    Value = "murderpeter@gmail.com",
                    });
                    this._loginParameters.Add("Password", new LoginParameter()
                    {
                        DisplayName = "密码",
                        IsRequired = true,
                        RequireEncryption = false,
                       //   Value = "likeni100",
                        Value = this.LoginConfiguration.Password,
                       // Value = "likeni100",
                    });

                }

                return this._loginParameters;
            }
        }

        public string MainEntryUrl
        {
            get { return "http://www.linkedin.com/uas/logout"; }
        }

        public string LoginUrl
        {
            get { return "https://www.linkedin.com/uas/login"; }
        }

        public string LoginSubmitUrl
        {
            get { return "https://www.linkedin.com/uas/login-submit"; }
        }

        public string LoginPostDataFormat
        {
            get { return "isJsEnabled=true&source_app=&session_key={0}&session_password={1}&signin=Sign%20In&session_redirect=&hr=&csrfToken={2}&sourceAlias={3}"; }
        }

        public string AdvanceProfileUrl
        {
            get { return "http://www.linkedin.com/search"; }
        }

        public string SearchProfileUrl
        {

            get
            {
                //return "http://www.linkedin.com/vsearch/p?school=Tsinghua%20University&postalCode={0}&openAdvancedForm=true&locationType=I&countryCode=us&distance=10&sortBy=R&rsid={1}&orig={2}&page_num={3}";

                //    return "http://www.linkedin.com/search/fpsearch?school=Tsinghua+University&searchLocationType=I&countryCode=us&postalCode={0}&distance=10&keepFacets=keepFacets&page_num={1}&&viewCriteria=1&sortCriteria=R";

                return "http://www.linkedin.com/search/hits";
            }
        }

        public string SearchPostDataFormat
        {
            get 
            {
              //  return "school=Tsinghua+University&searchLocationType=I&countryCode=us&postalCode={0}&distance=25&keepFacets=keepFacets&pplSearchOrigin=ADVS&viewCriteria=1&sortCriteria=R&page_num={1}&facetsOrder=CC%2CN%2CI%2CPC%2CL%2CFG%2CTE%2CFA%2CSE%2CP%2CCS%2CF%2CDR%2CG%2CED&openFacets=CC%2CN%2CI"; 
                return "school=Tsinghua+University&searchLocationType=I&countryCode=us&postalCode={0}&distance=" + this.LoginConfiguration.Distance + "&keepFacets=keepFacets&pplSearchOrigin=ADVS&viewCriteria=1&sortCriteria=" + this.LoginConfiguration.SortBy + "&page_num={1}&facetsOrder=CC%2CN%2CI%2CPC%2CL%2CFG%2CTE%2CFA%2CSE%2CP%2CCS%2CF%2CDR%2CG%2CED&openFacets=CC%2CN%2CI"; 
            }
        }

        public Dictionary<string, string> SearchProfileParamenters
        {
            get { throw new NotImplementedException(); }
        }
    }
}
