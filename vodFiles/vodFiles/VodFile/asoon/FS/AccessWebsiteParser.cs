namespace VodFile
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;

    public class AccessWebsiteParser
    {
        private static List<string> allowDomainList = ParseWebsiteConfiguration();

        public static bool CheckWebsite(string url)
        {
            string str = ConfigurationManager.AppSettings["allowdomainenabled"].Trim();
            bool flag = true;
            if (string.IsNullOrEmpty(str))
            {
                flag = false;
            }
            else
            {
                flag = str == "true";
            }
            if (flag && !string.IsNullOrEmpty(url.Trim()))
            {
                return (SearchWebsiteByURL(url) != -1);
            }
            return true;
        }

        public static List<string> GetDomainList()
        {
            return allowDomainList;
        }

        private static List<string> parseDomainXML(string xmlURL)
        {
            XmlDocument document = new XmlDocument();
            try
            {
                string filename = HttpContext.Current.Server.MapPath("~/" + xmlURL.Trim());
                document.Load(filename);
            }
            catch
            {
                return new List<string>();
            }
            XmlNode node = document.SelectSingleNode("domain");
            int count = node.ChildNodes.Count;
            List<string> list2 = new List<string>();
            for (int i = 0; i < count; i++)
            {
                Regex regex = new Regex("<!--(.*?)");
                if (!regex.IsMatch(node.ChildNodes[i].OuterXml))
                {
                    list2.Add(node.ChildNodes[i].InnerText.Trim());
                }
            }
            return list2;
        }

        private static List<string> ParseWebsiteConfiguration()
        {
            return parseDomainXML(ConfigurationManager.AppSettings["allowdomain"].Trim());
        }

        public static void ReloadDomainXML()
        {
            allowDomainList = parseDomainXML(ConfigurationManager.AppSettings["allowdomain"].Trim());
        }

        public static int SearchWebsiteByURL(string url)
        {
            ReloadDomainXML();
            if (allowDomainList.Count == 0)
            {
                return 0;
            }
            return allowDomainList.IndexOf(url.Trim());
        }

        public static void SetDomainList(List<string> domainlist)
        {
            allowDomainList.Clear();
            allowDomainList = domainlist;
        }
    }
}

