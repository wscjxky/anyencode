namespace VodFile
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Web;

    public class FLVStreaming : IHttpHandler
    {
        private static readonly byte[] _flvheader = HexToByte("464C5601010000000900000009");

        private bool checkKEY(HttpContext context)
        {
            string str = ConfigurationManager.AppSettings["safecode"].Trim();
            string str2 = ConfigurationManager.AppSettings["flvkey"].Trim();
            string str3 = context.Request.Params["key"];
            bool flag = false;
            if (str2 == "true")
            {
                if (str3.ToLower() == func.MD5(str + DateTime.Today.ToShortDateString()).ToLower())
                {
                    flag = true;
                }
                return flag;
            }
            return true;
        }

        private bool checkURL(HttpContext context)
        {
            return ((context.Request.UrlReferrer == null) || AccessWebsiteParser.CheckWebsite(context.Request.UrlReferrer.Host));
        }

        private static byte[] HexToByte(string hexString)
        {
            byte[] buffer = new byte[hexString.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 0x10);
            }
            return buffer;
        }

        protected void parseParameters(HttpContext context, long total, out int pos, out int endpos, out int len, out int[][] zones)
        {
            string str = context.Request.Params["start"];
            string str2 = context.Request.Params["end"];
            string str3 = context.Request.Params["zones"];
            if (string.IsNullOrEmpty(str2) || (str2 == "0"))
            {
                endpos = Convert.ToInt32(total);
            }
            else
            {
                endpos = Convert.ToInt32(str2);
                if (endpos <= 0)
                {
                    endpos = Convert.ToInt32(total);
                }
            }
            if (string.IsNullOrEmpty(str))
            {
                pos = 0;
            }
            else
            {
                pos = Convert.ToInt32(str);
            }
            if (string.IsNullOrEmpty(str3))
            {
                str3 = "-1,-2";
            }
            string[] strArray = str3.Split(new char[] { ';' });
            zones = new int[strArray.Length][];
            for (int i = 0; i < strArray.Length; i++)
            {
                string[] strArray2 = strArray[i].Split(new char[] { ',' });
                if (!string.IsNullOrEmpty(strArray2[0]) && !string.IsNullOrEmpty(strArray2[1]))
                {
                    if ((i == 0) && (strArray2[0] == "-1"))
                    {
                        strArray2[0] = ((int) pos).ToString();
                    }
                    if (strArray2[1] == "-2")
                    {
                        strArray2[1] = ((int) endpos).ToString();
                    }
                    int num2 = int.Parse(strArray2[0]);
                    int num3 = int.Parse(strArray2[1]);
                    if (num2 < num3)
                    {
                        if (pos < num2)
                        {
                            zones[i] = new int[] { num2, num3 };
                        }
                        else if (pos < num3)
                        {
                            zones[i] = new int[] { pos, num3 };
                        }
                        else
                        {
                            zones[i] = new int[2];
                        }
                    }
                }
            }
            if ((string.IsNullOrEmpty(str) && string.IsNullOrEmpty(str2)) && (zones.Length == 0))
            {
                len = Convert.ToInt32(total);
            }
            else
            {
                len = 0;
                if (zones.Length > 0)
                {
                    for (int j = 0; j < zones.Length; j++)
                    {
                        len += zones[j][1] - zones[j][0];
                    }
                    len += _flvheader.Length;
                }
                else
                {
                    len = Convert.ToInt32((int) (endpos - pos)) + _flvheader.Length;
                }
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Expires = 0;
            context.Response.Clear();
            context.Response.BufferOutput = false;
            FileStream stream = null;
            try
            {
                if (!this.checkURL(context) || !this.checkKEY(context))
                {
                    context.Response.End();
                }
                else
                {
                    int num;
                    int num2;
                    int num3;
                    int[][] numArray;
                    int index = 0;
                    int num5 = 0;
                    int num6 = 0;
                    string fileName = Path.GetFileName(context.Request.FilePath);
                    stream = new FileStream(context.Server.MapPath(fileName), FileMode.Open, FileAccess.Read, FileShare.Read, 0x4000, true);
                    this.parseParameters(context, stream.Length, out num, out num2, out num3, out numArray);
                    context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    context.Response.Cache.SetLastModified(DateTime.Now);
                    context.Response.AppendHeader("Content-Type", "video/x-flv");
                    context.Response.AppendHeader("Content-Length", num3.ToString());
                    context.Response.AppendHeader("Connection", "Keep-Alive");
                    if (num > 0)
                    {
                        context.Response.OutputStream.Write(_flvheader, 0, _flvheader.Length);
                        context.Response.OutputStream.Flush();
                    }
                    num5 = 0;
                    num6 = num2;
                    for (int i = 0; i < numArray.Length; i++)
                    {
                        if (((numArray[i] != null) && (numArray[i][0] >= 0)) && (numArray[i][0] < numArray[i][1]))
                        {
                            num5 = numArray[i][0];
                            num6 = numArray[i][1];
                            index = i;
                            break;
                        }
                    }
                    stream.Position = num5;
                    byte[] buffer = new byte[0x4000];
                    int count = stream.Read(buffer, 0, 0x4000);
                    while (count > 0)
                    {
                        if (context.Response.IsClientConnected)
                        {
                            if (stream.Position >= num2)
                            {
                                if (count > 0)
                                {
                                    context.Response.OutputStream.Write(buffer, 0, count);
                                }
                                count = -1;
                                return;
                            }
                            if (stream.Position >= num6)
                            {
                                index++;
                                if (index >= numArray.Length)
                                {
                                    count = -1;
                                    return;
                                }
                                if ((numArray[index] != null) && (numArray[index][0] != numArray[index][1]))
                                {
                                    num5 = numArray[index][0];
                                    num6 = numArray[index][1];
                                    stream.Position = num5;
                                }
                                else
                                {
                                    index++;
                                    continue;
                                }
                            }
                            if ((stream.Position + 0x4000) >= num6)
                            {
                                int num9 = Convert.ToInt32(stream.Position);
                                context.Response.OutputStream.Write(buffer, 0, count);
                                count = stream.Read(buffer, 0, num6 - num9);
                                context.Response.OutputStream.Flush();
                            }
                            else
                            {
                                context.Response.OutputStream.Write(buffer, 0, count);
                                context.Response.OutputStream.Flush();
                                count = stream.Read(buffer, 0, 0x4000);
                            }
                        }
                        else
                        {
                            count = -1;
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                context.Response.OutputStream.Flush();
                context.Response.OutputStream.Dispose();
                context.Response.OutputStream.Close();
                context.Response.Flush();
                context.Response.Clear();
                context.Response.Close();
                if (stream != null)
                {
                    stream.Flush();
                    stream.Dispose();
                    stream.Close();
                    stream = null;
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}

