namespace anyEncoder
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Management;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Security;
    using System.Windows.Forms;

    public class func
    {
        public static string ensn = string.Empty;
        private static Random rnd = new Random();
        public static string rndsn = string.Empty;

        public static void addLog(string logstr, string logfile)
        {
            try
            {
                FileInfo info = new FileInfo(logfile);
                if (info.Exists && (info.Length > 0x32000L))
                {
                    File.Delete(logfile);
                }
                logstr = DateTime.Now.ToString() + "\t" + logstr + Environment.NewLine;
                FileStream stream = new FileStream(logfile, FileMode.Append);
                StreamWriter writer = new StreamWriter(stream, Encoding.Default);
                writer.WriteLine(logstr);
                writer.Close();
                stream.Close();
            }
            catch
            {
            }
        }

        public static string ASPMD5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 0x10);
        }

        public static bool chkdog1()
        {
            if (DateTime.Now >= GetDateTime("2052-5-12"))
            {
                StringBuilder outKeyPath = new StringBuilder("", 260);
                if (FindPort_2(0, 0xc5380, -353976855, outKeyPath) != 0)
                {
                    return false;
                }
                string randomString = GetRandomString(8);
                int num = lstrlenA(randomString) + 1;
                if (num < 8)
                {
                    num = 8;
                }
                StringBuilder outstring = new StringBuilder("", num * 2);
                if (EncString(randomString, outstring, outKeyPath.ToString()) != 0)
                {
                    return false;
                }
                rndsn = randomString;
                ensn = outstring.ToString();
            }
            return true;
        }

        public static bool chkdog2()
        {
            if (DateTime.Now < GetDateTime("2052-5-12"))
            {
                return true;
            }
            SoftKey key = new SoftKey();
            return (ensn == key.StrEnc(rndsn, "9HBFF29RHGMAALQPCYJ171FJU174117A"));
        }

        public static bool chksn()
        {
            if (DateTime.Now < GetDateTime("2052-5-12"))
            {
                return true;
            }
            string xmlString = "<RSAKeyValue><Modulus>yTPoPUItqlr9BCSO8tQj8EbTlIrIfVpaXKJuqzCz5AS3IdzmqijdoYndiz6WN+5EC91OqfQlUIgE5M03vvIFfUZJ6U25MB7ANhQpqVUpnWxtAfAHFyo4zCqiLnLuE0GHlzPsC6zvg/fyL/nMlcxWRIhH3xKlX5xYTdE8YdpudWU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            string s = null;
            try
            {
                s = new IniFiles(Application.StartupPath + @"\config.ini").ReadString("system", "sn", "");
            }
            catch
            {
                return false;
            }
            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
            {
                byte[] rgbSignature = new byte[0];
                provider.FromXmlString(xmlString);
                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(provider);
                deformatter.SetHashAlgorithm("SHA1");
                try
                {
                    rgbSignature = Convert.FromBase64String(s);
                }
                catch
                {
                    return false;
                }
                byte[] inArray = new SHA1Managed().ComputeHash(Encoding.ASCII.GetBytes(getpccode()));
                string str3 = Convert.ToBase64String(inArray);
                return deformatter.VerifySignature(inArray, rgbSignature);
            }
        }

        public static void CopyDirectory(string SourceDirectory, string TargetDirectory)
        {
            DirectoryInfo info = new DirectoryInfo(SourceDirectory);
            DirectoryInfo info2 = new DirectoryInfo(TargetDirectory);
            if (info.Exists)
            {
                if (!info2.Exists)
                {
                    info2.Create();
                }
                FileInfo[] files = info.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    File.Copy(files[i].FullName, info2.FullName + @"\" + files[i].Name, true);
                }
                DirectoryInfo[] directories = info.GetDirectories();
                for (int j = 0; j < directories.Length; j++)
                {
                    CopyDirectory(directories[j].FullName, info2.FullName + @"\" + directories[j].Name);
                }
            }
        }

        public static void DelDir(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            DirectoryInfo[] directories = info.GetDirectories();
            FileInfo[] files = info.GetFiles();
            if (files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    File.Delete(path + @"\" + files[i].Name.ToString());
                }
            }
            if (directories.Length > 0)
            {
                for (int j = 0; j < directories.Length; j++)
                {
                    DelDir(path + @"\" + directories[j].Name.ToString());
                }
            }
        }

        [DllImport("SYUNEW3D.dll")]
        public static extern int EncString(string InString, StringBuilder outstring, string KeyPath);
        [DllImport("SYUNEW3D.dll")]
        public static extern int FindPort_2(int start, int in_data, int verf_data, StringBuilder OutKeyPath);
        public static bool GetBool(string strings)
        {
            return (!string.IsNullOrEmpty(strings) && (strings.ToLower() == "true"));
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
            if (!(string.IsNullOrEmpty(strings) || !DateTime.TryParse(strings.Trim(), out time)))
            {
                return time;
            }
            return DateTime.Now;
        }

        public static int GetInt(object obj)
        {
            string strings = (obj == DBNull.Value) ? "0" : obj.ToString();
            return GetInt(strings);
        }

        public static int GetInt(string strings)
        {
            if (!string.IsNullOrEmpty(strings))
            {
                int num;
                if (int.TryParse(strings.Trim(), out num))
                {
                    return num;
                }
                return -1;
            }
            return 0;
        }

        public static string GetMac()
        {
            try
            {
                ManagementObjectCollection objects = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration").Get();
                foreach (ManagementObject obj2 in objects)
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
            string mac = GetMac();
            if (mac.Length < 5)
            {
            }
            System.Security.Cryptography.MD5 md = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md.ComputeHash(Encoding.ASCII.GetBytes(mac))).ToLower();
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

        public static string GetServicePath(string sname)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service");
                foreach (ManagementObject obj2 in searcher.Get())
                {
                    if (sname == obj2["Name"].ToString())
                    {
                        return obj2["PathName"].ToString();
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public static bool GetServiceStat(string sname)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service");
                foreach (ManagementObject obj2 in searcher.Get())
                {
                    if (sname == obj2["Name"].ToString())
                    {
                        return obj2["Started"].Equals(true);
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static string GetString(object obj)
        {
            return GetString(obj, string.Empty);
        }

        public static string GetString(string strings)
        {
            return ((strings == null) ? string.Empty : strings.Trim());
        }

        public static string GetString(object obj, string value)
        {
            if ((obj == DBNull.Value) || (obj == null))
            {
                return value;
            }
            return obj.ToString();
        }

        public static string GetString(string strings, string value)
        {
            return (string.IsNullOrEmpty(strings) ? GetString(value) : strings.Trim());
        }

        public static void imgResize(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            Image image;
            try
            {
                image = Image.FromFile(originalImagePath);
            }
            catch (Exception)
            {
                return;
            }
            int num = width;
            int num2 = height;
            int x = 0;
            int y = 0;
            int num5 = image.Width;
            int num6 = image.Height;
            string str = mode.ToUpper();
            if ((str != null) && (str != "HW"))
            {
                if (str == "W")
                {
                    num2 = (image.Height * width) / image.Width;
                }
                else if (str == "H")
                {
                    num = (image.Width * height) / image.Height;
                }
                else if (str == "CUT")
                {
                    if (image.Width < image.Height)
                    {
                        int num7 = width;
                        width = height;
                        height = num7;
                        num = width;
                        num2 = height;
                    }
                    if ((((double) image.Width) / ((double) image.Height)) > (((double) num) / ((double) num2)))
                    {
                        num6 = image.Height;
                        num5 = (image.Height * num) / num2;
                        y = 0;
                        x = (image.Width - num5) / 2;
                    }
                    else
                    {
                        num5 = image.Width;
                        num6 = (image.Width * height) / num;
                        x = 0;
                        y = (image.Height - num6) / 2;
                    }
                }
                else if (str == "AUTO")
                {
                    if (image.Width > image.Height)
                    {
                        num2 = (image.Height * width) / image.Width;
                    }
                    else
                    {
                        num = (image.Width * height) / image.Height;
                    }
                }
            }
            if (num > image.Width)
            {
                num = image.Width;
                num2 = image.Height;
            }
            Image image2 = new Bitmap(num, num2);
            Graphics graphics = Graphics.FromImage(image2);
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.Clear(Color.Transparent);
            graphics.DrawImage(image, new Rectangle(0, 0, num, num2), new Rectangle(x, y, num5, num6), GraphicsUnit.Pixel);
            long[] numArray = new long[] { 90L };
            EncoderParameters encoderParams = new EncoderParameters();
            EncoderParameter parameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, numArray);
            encoderParams.Param[0] = parameter;
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo encoder = null;
            for (int i = 0; i < imageEncoders.Length; i++)
            {
                if (imageEncoders[i].FormatDescription.Equals("JPEG"))
                {
                    encoder = imageEncoders[i];
                    break;
                }
            }
            try
            {
                image2.Save(thumbnailPath, encoder, encoderParams);
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                image.Dispose();
                image2.Dispose();
                graphics.Dispose();
            }
        }

        public static void killall()
        {
            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.Length; i++)
            {
                if ("ffmpeg,mencoder,yamdi".IndexOf(processes[i].ProcessName.ToLower()) >= 0)
                {
                    try
                    {
                        processes[i].Kill();
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static string Left(string strings, int i)
        {
            if ((i != 0) && !string.IsNullOrEmpty(strings))
            {
                if (strings.Length > i)
                {
                    return strings.Trim().Substring(0, i);
                }
                return strings.Trim();
            }
            return string.Empty;
        }

        [DllImport("kernel32.dll")]
        public static extern int lstrlenA(string InString);
        public static string MD5(string str)
        {
            return MD5(str, 0x20);
        }

        public static string MD5(string str, int length)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            byte[] buffer = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(str));
            string str2 = BitConverter.ToString(buffer);
            if (length == 0x10)
            {
                str2 = BitConverter.ToString(buffer, 4, 8);
            }
            return str2.Replace("-", "").ToLower();
        }

        public static string readdog(int t)
        {
            string keyPath = new StringBuilder("", 260).ToString();
            short capacity = 3;
            StringBuilder outstring = new StringBuilder("", capacity);
            for (int i = 0; i < capacity; i++)
            {
                outstring.Append(0);
            }
            if (YReadString(outstring, 0, capacity, "ffffffff", "ffffffff", keyPath) == 0)
            {
                string[] strArray = outstring.ToString().Split(new char[] { '|' });
                if (strArray.Length == 2)
                {
                    return strArray[t];
                }
            }
            return "";
        }

        public static string Right(string strings, int i)
        {
            if ((i != 0) && !string.IsNullOrEmpty(strings))
            {
                if (strings.Length > i)
                {
                    return strings.Trim().Remove(0, strings.Length - i);
                }
                return strings.Trim();
            }
            return string.Empty;
        }

        public static string ShowSize(float size)
        {
            string format = "n0";
            float num = size / 1024f;
            string str2 = Left(num.ToString("n2"), 4) + "KB";
            if (size > 0f)
            {
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
            return "0KB";
        }

        public static string ShowSize(string size)
        {
            if (string.IsNullOrEmpty(size))
            {
                return "0KB";
            }
            return ShowSize((float) Convert.ToInt64(size));
        }

        [DllImport("SYUNEW3D.dll")]
        public static extern int YReadString(StringBuilder outstring, short Address, short mylen, string HKey, string LKey, string KeyPath);
    }
}

