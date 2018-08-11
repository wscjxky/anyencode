namespace VodFile
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Management;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Security;

    public class func
    {
        private static Random rnd = new Random();

        public static string ASPMD5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 0x10);
        }

        public static int DateDiff(string dateInterval, DateTime dateTime1, DateTime dateTime2)
        {
            int totalMinutes = 0;
            try
            {
                TimeSpan span = new TimeSpan(dateTime2.Ticks - dateTime1.Ticks);
                switch (dateInterval.ToLower())
                {
                    case "day":
                    case "d":
                        return (int) span.TotalDays;

                    case "hour":
                    case "h":
                        return (int) span.TotalHours;

                    case "minute":
                    case "n":
                        return (int) span.TotalMinutes;

                    case "second":
                    case "s":
                        return (int) span.TotalSeconds;

                    case "milliseconds":
                    case "ms":
                        return (int) span.TotalMilliseconds;
                }
                totalMinutes = (int) span.TotalMinutes;
            }
            catch
            {
            }
            return totalMinutes;
        }

        public static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string[] GetArray(string str, string regexStr)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new string[0];
            }
            if (str.Contains(regexStr))
            {
                return Regex.Split(str, Regex.Escape(regexStr), RegexOptions.IgnoreCase);
            }
            return new string[] { str };
        }

        public static long GetDriveFreeSpace(string driveName)
        {
            System.IO.DriveInfo drive = new System.IO.DriveInfo(driveName); 
            return drive.AvailableFreeSpace;//取得所在磁盘可用空间大小
        }
        public static string GetCpuID()
        {
            try
            {
                ManagementObjectCollection instances = new ManagementClass("Win32_Processor").GetInstances();
                string str = null;
                foreach (ManagementObject obj2 in instances)
                {
                    str = obj2.Properties["ProcessorId"].Value.ToString();
                    break;
                }
                return str;
            }
            catch
            {
                return "";
            }
        }

        public static DateTime GetDateTime(object obj)
        {
            return GetDateTime(obj.ToString());
        }

        public static DateTime GetDateTime(string strings)
        {
            DateTime time;
            if (!string.IsNullOrEmpty(strings) && DateTime.TryParse(strings.Trim(), out time))
            {
                return time;
            }
            return DateTime.Now;
        }

        public static string GetFlvFolder()
        {
            string path = "files/" + DateTime.Now.ToString("yyyy-MM", DateTimeFormatInfo.InvariantInfo);
            string strFilePath = HttpContext.Current.Server.MapPath(path);
            if (!Directory.Exists(strFilePath))
            {
                try
                {
                    Directory.CreateDirectory(strFilePath);
                }
                catch
                {
                }
            }
            return path;
        }

        public static string GetHiddenInput(string id)
        {
            return GetHiddenInput(id, "");
        }

        public static string GetHiddenInput(string id, string value)
        {
            return ("<input id=\"" + GetString(id, "ob_input") + "\"  name=\"" + GetString(id, "ob_input") + "\" type=\"hidden\" value=\"" + GetString(value) + "\" />");
        }

        public static string GetInput(string id, string name, string type, string value, string strChecked, string events)
        {
            string str = (strChecked == value) ? "checked" : string.Empty;
            return ("<input id=\"" + GetString(id, "ob_input") + "\" name =\"" + GetString(name, "ob_input") + "\" type=\"" + GetString(type, "text") + "\" value=\"" + value + "\" " + str + " " + events + ">");
        }

        public static int GetInt(object obj)
        {
            string strings = (obj == DBNull.Value) ? "0" : obj.ToString();
            return GetInt(strings);
        }

        public static int GetInt(string strings)
        {
            int num;
            if (string.IsNullOrEmpty(strings))
            {
                return 0;
            }
            if (int.TryParse(strings.Trim(), out num))
            {
                return num;
            }
            return -1;
        }

        public static string GetIp()
        {
            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null)
            {
                return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        }

        public static string GetMac()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
                foreach (ManagementObject obj2 in searcher.Get())
                {
                    if (obj2["IPEnabled"].ToString() == "True")
                    {
                        return obj2["MacAddress"].ToString();
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        public static string getpccode()
        {
            string s = GetCpuID() + GetMac();
            int length = s.Length;
            System.Security.Cryptography.MD5 md = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md.ComputeHash(Encoding.ASCII.GetBytes(s))).ToLower();
        }

        public static string GetRadioInput(string id, string value, string inputValue)
        {
            return GetInput(id, id, "radio", value, inputValue, "");
        }

        public static string GetRadioInput(string cname, string value1, string value2, string inputValue)
        {
            if ((value1 == "") || (value1 == null))
            {
                value1 = "否";
            }
            if ((value2 == "") || (value2 == null))
            {
                value2 = "是";
            }
            if ((inputValue == "") || (inputValue == null))
            {
                inputValue = "0";
            }
            string str = "<input type=\"radio\" name=\"" + cname + "\" id=\"" + cname + "\" value=\"0\" ";
            if (inputValue == "0")
            {
                str = str + "checked";
            }
            string str2 = str + " />" + value1 + "&nbsp;";
            str = str2 + "<input type=\"radio\" name=\"" + cname + "\" id=\"" + cname + "\" value=\"1\" ";
            if (inputValue == "1")
            {
                str = str + "checked";
            }
            return (str + " />" + value2);
        }

        public static char GetRandomChar(int i)
        {
            switch (i)
            {
                case 1:
                    return (char) rnd.Next(0x30, 0x3a);

                case 2:
                    return (char) rnd.Next(0x41, 0x5b);

                case 3:
                    return (char) rnd.Next(0x61, 0x7b);
            }
            return '0';
        }

        public static string GetRandomInt(int length)
        {
            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                builder.Append(GetRandomChar(1));
            }
            return builder.ToString();
        }

        public static string GetRandomString(int length)
        {
            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                builder.Append(GetRandomChar(rnd.Next(1, 4)));
            }
            return builder.ToString();
        }

        public static string GetString(object obj)
        {
            return GetString(obj, string.Empty);
        }

        public static string GetString(string strings)
        {
            if (strings != null)
            {
                return strings.Trim();
            }
            return string.Empty;
        }

        public static string GetString(object obj, string value)
        {
            if ((obj != DBNull.Value) && (obj != null))
            {
                return obj.ToString();
            }
            return value;
        }

        public static string GetString(string strings, string value)
        {
            if (!string.IsNullOrEmpty(strings))
            {
                return strings.Trim();
            }
            return GetString(value);
        }

        public static int GetStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        public static string GetTextInput(string id, string size, string maxlength, string value, string events)
        {
            return ("<input type=\"text\" id=\"" + GetString(id, "ob_input") + "\" name=\"" + GetString(id, "ob_input") + "\" size=\"" + GetString(size, "30") + "\" maxlength=\"" + GetString(maxlength, "30") + "\" value=\"" + value + "\" " + events + " />");
        }

        public static string GetUrlPath()
        {
            string str = "";
            if (HttpContext.Current.Request.ServerVariables["HTTPS"] != "on")
            {
                str = "http://";
            }
            else
            {
                str = "https://";
            }
            str = str + HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
            if (HttpContext.Current.Request.ServerVariables["SERVER_PORT"] != "80")
            {
                str = str + ":" + HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            }
            str = str + HttpContext.Current.Request.ServerVariables["URL"];
            string[] strArray = HttpContext.Current.Request.Url.PathAndQuery.Split(new char[] { '?' });
            string oldValue = strArray[0].Substring(strArray[0].LastIndexOf('/') + 1);
            return str.Replace(oldValue, "");
        }

        public static string GetUserIp()
        {
            return HttpContext.Current.Request.UserHostAddress;
        }

        public static string Htm2Js(string htm)
        {
            if (htm != null)
            {
                return htm.Replace(@"\", @"\\").Replace("'", "&acute;").Replace("\n", @"\n").Replace("\r", "").Replace("\"", "\\\"");
            }
            return string.Empty;
        }

        public static string HtmlEncode(string strings)
        {
            if (!string.IsNullOrEmpty(strings))
            {
                return strings.Trim().Replace(">", "&gt").Replace("<", "&lt").Replace("\n", "<br />").Replace("\"", "&quot").Replace("&", "&amp;").Replace(" ", "&nbsp;").Replace("\r", "");
            }
            return string.Empty;
        }

        public static string HtmlReplace(string strings)
        {
            if (!string.IsNullOrEmpty(strings))
            {
                return strings.Trim().Replace(">", "&gt").Replace("<", "&lt");
            }
            return string.Empty;
        }

        public static string Left(string strings, int i)
        {
            if (i == 0)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(strings))
            {
                return string.Empty;
            }
            if (strings.Length > i)
            {
                return strings.Trim().Substring(0, i);
            }
            return strings.Trim();
        }

        public static string MD5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        }

        public static string ProtectSql(string strings)
        {
            if (strings != null)
            {
                return strings.Trim().Replace("'", "''");
            }
            return string.Empty;
        }

        public static string Right(string strings, int i)
        {
            if (i == 0)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(strings))
            {
                return string.Empty;
            }
            if (strings.Length > i)
            {
                return strings.Trim().Remove(0, strings.Length - i);
            }
            return strings.Trim();
        }

        public static string ShowSize(float size)
        {
            string format = "n0";
            float num = size / 1024f;
            string str2 = Left(num.ToString("n2"), 4) + "KB";
            if (size <= 0f)
            {
                return "0KB";
            }
            if (size > 1024f)
            {
                size /= 1024f;
                str2 = size.ToString(format) + "KB";
            }
            if (size > 1024f)
            {
                size /= 1024f;
                str2 = size.ToString(format) + "MB";
            }
            if (size > 1024f)
            {
                size /= 1024f;
                str2 = size.ToString(format) + "GB";
            }
            if (size > 1024f)
            {
                size /= 1024f;
                str2 = size.ToString(format) + "TB";
            }
            if (size > 1024f)
            {
                size /= 1024f;
                str2 = size.ToString(format) + "PB";
            }
            if (size > 1024f)
            {
                size /= 1024f;
                str2 = size.ToString(format) + "EB";
            }
            return str2;
        }

        public static string ShowSize(string size)
        {
            if (string.IsNullOrEmpty(size))
            {
                return "0KB";
            }
            return ShowSize((float) Convert.ToInt64(size));
        }

        public static void wlog(string str)
        {
            FileStream stream = new FileStream(HttpContext.Current.Server.MapPath("~/log") + @"\log.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(stream);
            writer.Flush();
            writer.BaseStream.Seek(0, SeekOrigin.Begin);
            writer.Write(str);
            writer.Flush();
            writer.Close();
        }
    }
}

