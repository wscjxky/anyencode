using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using System.Security.AccessControl;


namespace VodFile
{
    public class FileUpload
    {
        private static Random rnd = new Random();
        /// <summary>
        /// 按时间年月日创建文件夹
        /// </summary>
        /// <returns>子文件夹绝对路径</returns>
        public static string CreatFolder()
        {
            string str = HttpContext.Current.Server.MapPath(".");
            //判断根目录是否存在，不存在创建
            DirectoryInfo dr = new DirectoryInfo(str);
            if (!Directory.Exists(str + "\\tmpfiles"))
            {
                dr.Create();
            }
            //else
            //{
            //   str =str+"\\"+ DateTime.Now.ToString("yyyy-MM-dd");
            //   DirectoryInfo sonFolder = new DirectoryInfo(str);
            //   if (!sonFolder.Exists)
            //   {
            //       sonFolder.Create();
            //   }
            //}
            return str;
        }
        /// <summary>
        /// 大小计算
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string ShowSize(float size)
        {
            string str2 = "0";
            if (size <= 0f)
            {
                return "0";
            }
            if (size > 1024f)
            {
                size /= 1024f;
                str2 = size.ToString();
            }
            return str2;
        }
        public static char GetRandomChar(int i)
        {
            switch (i)
            {
                case 1:
                    return (char)rnd.Next(0x30, 0x3a);

                case 2:
                    return (char)rnd.Next(0x41, 0x5b);

                case 3:
                    return (char)rnd.Next(0x61, 0x7b);
            }
            return '0';
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
        //产生 16位随机字符串 
        //无重复的filecode
        public static string GenericRandom16Filecode(string filename)
        {
            string filecode = "";
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));//1970 1 1 0 0 0 0
            DateTime nowTime = DateTime.Now;
            long unixTime = (long)Math.Round((nowTime - startTime).TotalMilliseconds, MidpointRounding.AwayFromZero);//相减两个时间 获取时间戳
            for (long i = 0; ; i += (unixTime << 1))
            {
                if (filecode.Length >= 10)
                    break;
                long val = i ^ unixTime;
                string dexVal = Convert.ToString(val, 16);
                Random rn = new Random();
                int pos = rn.Next(0, dexVal.Length - 1);
                filecode = filecode + dexVal.Substring(rn.Next(0, 2), pos);
            }
            if (filecode.Length == 16)
                return filecode;
            if (filecode.Length > 16)
            {
                filecode = filecode.Substring(0, 10);
            }
            string str = "0abc1defg3hij2kl4mn5op6qr7st8uv9wxyz";
            Random r = new Random();
            for (; ; )
            {
                if (filecode.Length == 16)
                    return GetMd5_16(filecode.ToString() + filename).ToLower().ToString();
                filecode = GetMd5_16((filecode + str[r.Next(0, 35)]).ToString() + filename).ToLower().ToString();
            }
        }
        public static string GetMd5_16(string ConvertString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(System.Text.UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
        public static string GetFlvFolder(string root)
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

    }
}