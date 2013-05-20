using BolePlus.Client.GoogleCache;
using BolePlus.Client.LinkedIn;
using BolePlus.Client.LinkedIn.DataAccessor;
using BolePlus.Client.Proxy;
using System;
using System.Xml;

namespace WebSiteConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                CreateMenu();
                try
                {
                    DoOperation();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static void CreateMenu()
        {
            Console.WriteLine();
            Console.WriteLine("\t菜单选择");
            Console.WriteLine("\t0.通过 Google Cache 搜索人才(文件)");
            Console.WriteLine("\t1.通过 Google Cache 搜索人才(使用代理)");
            Console.WriteLine("\t2.通过 Google Cache 搜索人才(不用代理)");
            Console.WriteLine("\t3.通过 Google Cache 搜索人才（测试代理）");
            Console.WriteLine("\t4.通过Premium Account 搜索LinkedIn Id");
            Console.WriteLine("\t5.通过LinkIn ID列表搜索人才资料");
            Console.WriteLine("\t6.解析人才资料");
            Console.WriteLine("\t7.拆分LinkedIn Id列表(700个为以单位)");
            Console.WriteLine("\t8.解析通过Premium Account获取ProfileID文件");
            Console.WriteLine("\t9.根据State拆分PostaCode");
            Console.WriteLine("\t10.整合Postal Code，更新状态");
            //Console.WriteLine("\t6.获取国内Proxy列表");
            //Console.WriteLine("\t7.MongoDb测试");
            //Console.WriteLine("\t8.MongoDb Clear Up");
          
            Console.WriteLine("\t11.通过People You May Know获取LinkedIn 人才ViewProfileUrl列表并且Merge");
            Console.WriteLine("\t12.通过College获取LinkedIn 人才ID列表");
            Console.Write("请输入您的选择: ");
        }

        private static void DoOperation()
        {
            GoogleCacheWebSystemProvider dataProvider = new GoogleCacheWebSystemProvider();
            LinkedInWebSystemProvider linkedInProvider = new LinkedInWebSystemProvider();

            string input = Console.ReadLine().Trim();
            int index = Convert.ToInt32(input);
            switch (index)
            {
                case 0:
                    dataProvider.GetProfilesEx();
                    break;
                case 1:
                    dataProvider.GetProfiles();
                    break;
                case 2:
                    dataProvider.GetProfileWithoutProxy();
                    break;
                case 3:
                    dataProvider.GetProfilesExWithProxy();
                    break;
                case 4:
                    LinkedInWebSystemProvider dataProvider4 = new LinkedInWebSystemProvider();
                    dataProvider4.GetProfileIds();
                    break;
                case 5:
                    linkedInProvider.GetProfiles();
                    break;
                case 6:
                    linkedInProvider.ProfileFileHandler();
                    break;
                case 7:
                    linkedInProvider.SpliteProfileConfiguration();
                    break;
                case 8:
                    linkedInProvider.ProfileIdFileHandler();
                    linkedInProvider.MergePostalFromDifferentState();
                    break;
                case 9:
                    linkedInProvider.SplitPostalCode();
                    break;
                case 10:
                    linkedInProvider.MergePostalFromDifferentState();
                    break;
                //case 6:
                //    ProxyWebSystemProvider proxyProvider = new ProxyWebSystemProvider();
                //    proxyProvider.GetProxyList();
                //    break;
                //case 7:                   
                //    linkedInProvider.GetProfilesTestForJson();
                //    break;
                //case 8:
                //    ProfileDataAccessor.Remove();
                //    break;
                case 12:
                    //linkedInProvider.GetProfileIdsFromCollege();
                    linkedInProvider.MergeProfileFromCollegeToCollege();
                    break;
                case 11:
                    linkedInProvider.GetProfileFromPeopleYouMayKnow();
                    linkedInProvider.MergeProfileFromPeopleYouMayKnowToCollege();
                    break;
                default:
                    break;
            }
        }

    }
}