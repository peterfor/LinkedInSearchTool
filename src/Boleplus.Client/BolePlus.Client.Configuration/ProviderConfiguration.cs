using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Configuration;
using System.Xml;

namespace BolePlus.Client.Configuration
{
    public class ProviderConfiguration
    {
        public static T GetConfigurationEntity<T>(ConfigurationType type) where T : class
        {
            string path = string.Empty;
            string fileName = string.Empty;
            T instance = null;
            switch (type)
            {
                case ConfigurationType.USPostalCode:
                    fileName = "UsPostCode.xml";
                    break;
                case ConfigurationType.USStateCode:
                    fileName = "USStateCode.xml";
                    break;
                case ConfigurationType.Proxy:
                    fileName = "Proxy.xml";
                    break;
                case ConfigurationType.USCity:
                    fileName = "USCity.xml";
                    break;
                case ConfigurationType.Profile:
                    fileName = "ProfileIds2.xml";
                    break;
                case ConfigurationType.ProfileFromCollege:
                    fileName = "ProfileIdsFromCollege.xml";
                    break;
                case ConfigurationType.ProfileFromPeopleYouMayKnowResult:
                    fileName = "ProfileIdsFromCollegeResult.xml";
                    break;
                case ConfigurationType.ProfileFromCollege2:
                    fileName = "ProfileIdsFromCollege2.xml";
                    break;
                default: break;
            }

            path = string.Format(string.Format(@"{0}\{1}\{2}", Environment.CurrentDirectory, "config", fileName));
            try
            {
                if (File.Exists(path))
                {
                    string content = File.ReadAllText(path);
                    content = content.Replace("&amp;", "&");
                    using (FileStream stream = File.Open(path, FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        instance = (T)serializer.Deserialize(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return instance;
        }

        public static T GetConfigurationEntity<T>(string path) where T : class
        {
            T instance = null;
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    instance = (T)serializer.Deserialize(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return instance;
        }

        public static void SerializeConfigurationEntityToXml<T>(ConfigurationType type, object obj) where T : class
        {
            string path = string.Empty;
            string fileName = string.Empty;
            switch (type)
            {
                case ConfigurationType.USPostalCode:
                    fileName = "UsPostCode.xml";
                    break;
                case ConfigurationType.USStateCode:
                    fileName = "USStateCode.xml";
                    break;
                case ConfigurationType.Proxy:
                    fileName = "Proxy2.xml";
                    break;
                case ConfigurationType.USCity:
                    fileName = "USCity.xml";
                    break;
                case ConfigurationType.Profile:
                    fileName = "ProfileIds2.xml";
                    break;
                case ConfigurationType.ProfileFailure:
                    fileName = "FailedProfileIds.xml";
                    break;
                case ConfigurationType.ProfileFromCollege:
                    fileName = "ProfileIdsFromCollege.xml";
                    break;
                case ConfigurationType.ProfileFromPeopleYouMayKnowResult:
                    fileName = "ProfileIdsFromCollegeResult.xml";
                    break;
                default: break;
            }

            path = string.Format(string.Format(@"{0}\{1}\{2}", Environment.CurrentDirectory, "config", fileName));
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (XmlWriter writer = XmlWriter.Create(path))
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, obj, null);

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static void SerializeConfigurationEntityToXml<T>(object obj, string fileName,bool isOverride=false) where T : class
        {
            var path = string.Format(string.Format(@"{0}\{1}\{2}.xml", Environment.CurrentDirectory, "config\\profileId", fileName));
            if(isOverride)
            {
                path = fileName;
            }
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (XmlWriter writer = XmlWriter.Create(path))
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, obj, null);

                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static ProxyConfiguration ProxyConfiguration
        {
            get
            {
                return ProviderConfiguration.GetConfigurationEntity<ProxyConfiguration>(ConfigurationType.Proxy);
            }
        }

        public static PostalCodeConfiguration PostalCodeConfiguration
        {
            get
            {
                return ProviderConfiguration.GetConfigurationEntity<PostalCodeConfiguration>(ConfigurationType.USPostalCode);
            }
        }

        public static CityConfiguration CityConfiguration
        {
            get
            {
                return ProviderConfiguration.GetConfigurationEntity<CityConfiguration>(ConfigurationType.USCity);
            }
        }
    }
}