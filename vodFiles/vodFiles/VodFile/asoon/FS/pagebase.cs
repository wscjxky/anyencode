namespace VodFile
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI;
    using System.Configuration;

    public class pagebase : Page
    {
        //public static string scode = func.MD5(ConfigurationManager.AppSettings["safecode"].Trim());
        //public static string serverurl = ConfigurationManager.AppSettings["serverurl"].Trim();

        public static void EchoJs(string js)
        {
            HttpContext.Current.Response.Write("<script>" + js + "</script>");
            HttpContext.Current.Response.End();
        }

        public static void EchoJsErr(string errstr)
        {
            HttpContext.Current.Response.Write("<script>alert('" + errstr + "');history.back();</script>");
            HttpContext.Current.Response.End();
        }

        public static string Get(string strings)
        {
            string str = HttpContext.Current.Request.QueryString[strings];
            if (str != null)
            {
                return str.Trim();
            }
            return string.Empty;
        }

        //public static string[] get_uinfo()
        //{
        //    string webSite = serverurl + "?action=get_uinfo&u=" + uname + "&p=" + upwd + "&i=" + uid + "&scode=" + scode + "&filetype=" + Get("filetype");
        //    string str2 = send(webSite);
        //    if (string.IsNullOrEmpty(str2))
        //    {
        //        return new string[] { "0", "未获取到验证数据" };
        //    }
        //    string[] strArray = Regex.Split(str2, @"\$\$\$");
        //    if (strArray[0] == "0")
        //    {
        //        HttpContext.Current.Response.Write(strArray[0] + "错误url：" + webSite);
        //        HttpContext.Current.Response.End();
        //    }
        //    return strArray;
        //}

        public static string NowFile()
        {
            return HttpContext.Current.Request.Path.ToString();
        }

        public static string NowUrl()
        {
            return HttpUtility.UrlEncode(HttpContext.Current.Request.Url.AbsoluteUri.ToString());
        }

        //public static string[] oencoder_get_uinfo()
        //{
        //    return Regex.Split(send(serverurl + "?action=get_uinfo&u=" + uname + "&p=" + func.MD5(upwd) + "&i=" + uid + "&scode=" + scode), @"\$\$\$");
        //}

        public static string Post(string strings)
        {
            string str = HttpContext.Current.Request.Form[strings];
            if (str != null)
            {
                return HttpUtility.UrlDecode(str.Trim());
            }
            return string.Empty;
        }

        public static string send(string WebSite)
        {
            WebSite = WebSite + "&rnd=" + func.GetRandomString(10);
            int num = 10;
            HttpWebResponse response = null;
            string s = "";
            Encoding encoding = Encoding.UTF8;
            if (num == 0)
            {
                num = 0x2710;
            }
            else
            {
                num *= 0x3e8;
            }
            byte[] bytes = encoding.GetBytes(s);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(WebSite);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = num;
            request.ContentLength = bytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            try
            {
                response = (HttpWebResponse) request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string uid
        {
            get
            {
                return Get("i");
            }
        }

        public static string uname
        {
            get
            {
                return Get("u").Trim();
            }
        }

        public static string upwd
        {
            get
            {
                return Get("p");
            }
        }
    }
}

